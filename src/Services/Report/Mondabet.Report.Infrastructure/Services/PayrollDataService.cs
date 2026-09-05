using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Mondabet.Report.Application.Interfaces;

namespace Mondabet.Report.Infrastructure.Services;

/// <summary>
/// Cross-service data fetch for the Payroll report — same pattern as AttendanceDataService
/// (forward the caller's own bearer token; the caller here is always a CompanyAdmin whose
/// JWT already carries a single "tid", so no X-Tenant-Id override is needed, unlike
/// TenantDataService's SuperAdmin-across-all-tenants case).
/// </summary>
public class PayrollDataService : IPayrollDataService
{
    private const int CheckInPageSize = 200;
    private const int MaxCheckInPages = 50; // safety cap: up to 50 * 200 = 10,000 rows/month

    private static readonly System.Text.Json.JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PayrollDataService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IReadOnlyList<PayrollEmployeeInfo>> GetEmployeesAsync(CancellationToken ct = default)
    {
        var client = CreateAuthorizedClient("EmployeeApi");
        var response = await client.GetAsync("/api/v1/employees?page=1&size=5000", ct);
        if (!response.IsSuccessStatusCode) return Array.Empty<PayrollEmployeeInfo>();

        var envelope = await response.Content
            .ReadFromJsonAsync<RemoteApiResponse<RemotePagedResult<RemoteEmployeeDto>>>(JsonOptions, ct);
        var items = envelope?.Data?.Items ?? new List<RemoteEmployeeDto>();

        return items
            .Where(e => e.IsActive)
            .Select(e => new PayrollEmployeeInfo(
                e.Id, e.FullNameEn, e.FullNameAr, e.EmployeeNumber, e.DepartmentId, e.ShiftId,
                e.BaseSalary, e.HourlyRate))
            .ToList();
    }

    public async Task<IReadOnlyList<PayrollDepartmentInfo>> GetDepartmentsAsync(CancellationToken ct = default)
    {
        var client = CreateAuthorizedClient("EmployeeApi");
        var response = await client.GetAsync("/api/v1/departments", ct);
        if (!response.IsSuccessStatusCode) return Array.Empty<PayrollDepartmentInfo>();

        var envelope = await response.Content
            .ReadFromJsonAsync<RemoteApiResponse<List<RemoteDepartmentDto>>>(JsonOptions, ct);
        var items = envelope?.Data ?? new List<RemoteDepartmentDto>();
        return items.Select(d => new PayrollDepartmentInfo(d.Id, d.Name)).ToList();
    }

    public async Task<IReadOnlyList<PayrollShiftInfo>> GetShiftsAsync(CancellationToken ct = default)
    {
        var client = CreateAuthorizedClient("AttendanceApi");
        var response = await client.GetAsync("/api/v1/shifts", ct);
        if (!response.IsSuccessStatusCode) return Array.Empty<PayrollShiftInfo>();

        var shifts = await response.Content.ReadFromJsonAsync<List<RemoteShiftDto>>(JsonOptions, ct);
        return shifts?
            .Select(s => new PayrollShiftInfo(s.Id, s.StartTime, s.EndTime, s.DaysOfWeekJson ?? "[]"))
            .ToList() ?? new List<PayrollShiftInfo>();
    }

    public async Task<IReadOnlyList<PayrollCheckInInfo>> GetCheckInsAsync(
        DateTime from, DateTime to, CancellationToken ct = default)
    {
        var client = CreateAuthorizedClient("AttendanceApi");
        var all = new List<PayrollCheckInInfo>();

        for (var page = 1; page <= MaxCheckInPages; page++)
        {
            var url = $"/api/v1/checkins?from={Uri.EscapeDataString(from.ToString("o"))}" +
                      $"&to={Uri.EscapeDataString(to.ToString("o"))}" +
                      $"&page={page}&size={CheckInPageSize}";

            var response = await client.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode) break;

            var paged = await response.Content
                .ReadFromJsonAsync<RemotePagedResult<RemoteCheckInDto>>(JsonOptions, ct);
            if (paged is null || paged.Items.Count == 0) break;

            all.AddRange(paged.Items.Select(c =>
                new PayrollCheckInInfo(c.EmployeeId, c.ShiftId, c.CheckInTime, c.CheckOutTime,
                    (PayrollCheckStatus)c.CheckInStatus)));

            if (all.Count >= paged.TotalCount || paged.Items.Count < CheckInPageSize) break;
        }

        return all;
    }

    private HttpClient CreateAuthorizedClient(string name)
    {
        var client = _httpClientFactory.CreateClient(name);
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrEmpty(authHeader) && AuthenticationHeaderValue.TryParse(authHeader, out var parsed))
            client.DefaultRequestHeaders.Authorization = parsed;
        return client;
    }

    private record RemoteEmployeeDto(
        Guid Id, string FullNameEn, string FullNameAr, string? EmployeeNumber,
        Guid DepartmentId, Guid? ShiftId, bool IsActive, decimal? BaseSalary, decimal? HourlyRate);
    private record RemoteDepartmentDto(Guid Id, string Name);
    private record RemoteShiftDto(Guid Id, TimeOnly StartTime, TimeOnly EndTime, string? DaysOfWeekJson);
    private record RemoteCheckInDto(Guid EmployeeId, Guid ShiftId, DateTime CheckInTime, DateTime? CheckOutTime, int CheckInStatus);
    private record RemotePagedResult<T>(List<T> Items, int TotalCount, int Page, int PageSize);
    private record RemoteApiResponse<T>(T? Data, object? Error, string TraceId);
}
