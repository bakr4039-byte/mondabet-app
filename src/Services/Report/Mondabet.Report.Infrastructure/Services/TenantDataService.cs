using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Mondabet.Report.Application.Interfaces;

namespace Mondabet.Report.Infrastructure.Services;

/// <summary>
/// Real cross-service data fetch for the SuperAdmin company report, replacing
/// StubTenantDataService's always-empty list. Calls the Tenant service for the tenant list,
/// then — per tenant — the Employee and Attendance services for a headcount and a
/// "last activity" timestamp.
///
/// Employee/Attendance are per-tenant-schema services gated by TenantMiddleware, which reads
/// the tenant either from the caller's own JWT "tid" claim or, when absent (exactly the
/// SuperAdmin case here — a SuperAdmin's token isn't scoped to one tenant), from an
/// "X-Tenant-Id" header. So each per-tenant call below sets that header explicitly.
/// </summary>
public class TenantDataService : ITenantDataService
{
    private static readonly System.Text.Json.JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantDataService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IReadOnlyList<TenantSummaryRow>> GetTenantSummaryRowsAsync(CancellationToken ct = default)
    {
        var tenantClient = CreateAuthorizedClient("TenantApi");
        var tenants = await GetTenantsAsync(tenantClient, ct);

        var rows = new List<TenantSummaryRow>(tenants.Count);
        foreach (var t in tenants)
        {
            var employeeClient = CreateAuthorizedClient("EmployeeApi", t.Id);
            var attendanceClient = CreateAuthorizedClient("AttendanceApi", t.Id);

            var employeeCount = await GetEmployeeCountAsync(employeeClient, ct);
            var lastActivity = await GetLastActivityAsync(attendanceClient, ct);

            rows.Add(new TenantSummaryRow(t.Code, t.CompanyName, employeeCount, lastActivity));
        }

        return rows;
    }

    private HttpClient CreateAuthorizedClient(string name, Guid? tenantId = null)
    {
        var client = _httpClientFactory.CreateClient(name);

        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrEmpty(authHeader) && AuthenticationHeaderValue.TryParse(authHeader, out var parsed))
            client.DefaultRequestHeaders.Authorization = parsed;

        if (tenantId.HasValue)
            client.DefaultRequestHeaders.Add("X-Tenant-Id", tenantId.Value.ToString());

        return client;
    }

    private static async Task<List<RemoteTenantDto>> GetTenantsAsync(HttpClient client, CancellationToken ct)
    {
        var response = await client.GetAsync("/api/v1/tenants?page=1&size=1000", ct);
        if (!response.IsSuccessStatusCode) return new List<RemoteTenantDto>();

        var envelope = await response.Content
            .ReadFromJsonAsync<RemoteApiResponse<RemotePagedResult<RemoteTenantDto>>>(JsonOptions, ct);
        return envelope?.Data?.Items ?? new List<RemoteTenantDto>();
    }

    private static async Task<int> GetEmployeeCountAsync(HttpClient client, CancellationToken ct)
    {
        // Only the count is needed, not the rows — ask for a single row per page.
        var response = await client.GetAsync("/api/v1/employees?page=1&size=1", ct);
        if (!response.IsSuccessStatusCode) return 0;

        var envelope = await response.Content
            .ReadFromJsonAsync<RemoteApiResponse<RemotePagedResult<object>>>(JsonOptions, ct);
        return envelope?.Data?.TotalCount ?? 0;
    }

    private static async Task<DateTime?> GetLastActivityAsync(HttpClient client, CancellationToken ct)
    {
        // No date filter + page=1&size=1 returns just the single most recent check-in
        // (the repository already orders by CheckInTime descending).
        var response = await client.GetAsync("/api/v1/checkins?page=1&size=1", ct);
        if (!response.IsSuccessStatusCode) return null;

        var paged = await response.Content
            .ReadFromJsonAsync<RemotePagedResult<RemoteCheckInDto>>(JsonOptions, ct);
        return paged?.Items.FirstOrDefault()?.CheckInTime;
    }

    private record RemoteTenantDto(Guid Id, string Code, string CompanyName);
    private record RemoteCheckInDto(DateTime CheckInTime);
    private record RemotePagedResult<T>(List<T> Items, int TotalCount, int Page, int PageSize);
    private record RemoteApiResponse<T>(T? Data, object? Error, string TraceId);
}
