using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Mondabet.Leave.Application.Interfaces;

namespace Mondabet.Leave.Infrastructure.Services;

/// Same named-HttpClientFactory + bearer-forwarding pattern already established by
/// Report's PayrollDataService and Attendance's EmployeeLookupService for cross-service
/// reads: the caller's own Authorization header is forwarded so the Employee service
/// sees the original CompanyAdmin's token.
public class EmployeeLookupService : IEmployeeLookupService
{
    private static readonly System.Text.Json.JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EmployeeLookupService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<EmployeeSummary?> GetByIdAsync(Guid employeeId, CancellationToken ct = default)
    {
        var client = CreateAuthorizedClient("EmployeeApi");
        var response = await client.GetAsync($"/api/v1/employees/{employeeId}", ct);
        if (!response.IsSuccessStatusCode) return null;

        var envelope = await response.Content
            .ReadFromJsonAsync<RemoteApiResponse<RemoteEmployeeDto>>(JsonOptions, ct);
        var e = envelope?.Data;
        return e is null ? null : ToSummary(e);
    }

    public async Task<IReadOnlyList<EmployeeSummary>> GetByDepartmentAsync(Guid departmentId, CancellationToken ct = default)
    {
        var client = CreateAuthorizedClient("EmployeeApi");
        var response = await client.GetAsync("/api/v1/employees?page=1&size=5000", ct);
        if (!response.IsSuccessStatusCode) return Array.Empty<EmployeeSummary>();

        var envelope = await response.Content
            .ReadFromJsonAsync<RemoteApiResponse<RemotePagedResult<RemoteEmployeeDto>>>(JsonOptions, ct);
        var items = envelope?.Data?.Items ?? new List<RemoteEmployeeDto>();

        return items
            .Where(e => e.DepartmentId == departmentId)
            .Select(ToSummary)
            .ToList();
    }

    private static EmployeeSummary ToSummary(RemoteEmployeeDto e) =>
        new(e.Id, e.FullNameAr, e.FullNameEn, e.JobTitle, e.DepartmentId, e.IsActive);

    private HttpClient CreateAuthorizedClient(string name)
    {
        var client = _httpClientFactory.CreateClient(name);
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrEmpty(authHeader) && AuthenticationHeaderValue.TryParse(authHeader, out var parsed))
            client.DefaultRequestHeaders.Authorization = parsed;
        return client;
    }

    private record RemoteEmployeeDto(
        Guid Id, string FullNameAr, string FullNameEn, string JobTitle, Guid DepartmentId, bool IsActive);
    private record RemotePagedResult<T>(List<T> Items, int TotalCount, int Page, int PageSize);
    private record RemoteApiResponse<T>(T? Data, object? Error, string TraceId);
}
