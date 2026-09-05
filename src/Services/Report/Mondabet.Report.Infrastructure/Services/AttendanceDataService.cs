using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Mondabet.Report.Application.Interfaces;

namespace Mondabet.Report.Infrastructure.Services;

/// <summary>
/// Real cross-service data fetch, replacing the previous stub that always returned an
/// empty list (meaning attendance reports never had any actual data in them). Talks to
/// the Attendance service (check-ins + shifts) and the Employee service (names/Iqama) over
/// HTTP, using named clients whose base addresses come from configuration ("Services:...").
/// The caller's own bearer token is forwarded downstream so those services' own
/// [Authorize]/policy checks pass — no separate service-account token is needed.
/// </summary>
public class AttendanceDataService : IAttendanceDataService
{
    private const int MaxCheckInPages = 20; // safety cap: up to 20 * 200 = 4000 rows per report
    private const int CheckInPageSize = 200;

    private static readonly System.Text.Json.JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AttendanceDataService(
        IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IReadOnlyList<AttendanceRow>> GetAttendanceRowsAsync(
        Guid? employeeId, DateTime from, DateTime to, CancellationToken ct = default)
    {
        var attendanceClient = CreateAuthorizedClient("AttendanceApi");
        var employeeClient = CreateAuthorizedClient("EmployeeApi");

        var shiftNamesById = await GetShiftNamesAsync(attendanceClient, ct);
        var checkIns = await GetCheckInsAsync(attendanceClient, employeeId, from, to, ct);
        var employeesById = await GetEmployeeLookupAsync(employeeClient, ct);

        var rows = new List<AttendanceRow>(checkIns.Count);
        foreach (var c in checkIns)
        {
            var (name, iqama) = employeesById.TryGetValue(c.EmployeeId, out var e)
                ? (e.FullNameEn, e.Iqama)
                : (c.EmployeeId.ToString(), string.Empty);
            var shiftName = shiftNamesById.GetValueOrDefault(c.ShiftId, string.Empty);

            rows.Add(new AttendanceRow(name, iqama, c.CheckInTime, c.CheckOutTime, c.IsWithinGeofence, shiftName));
        }

        return rows;
    }

    private HttpClient CreateAuthorizedClient(string name)
    {
        var client = _httpClientFactory.CreateClient(name);

        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrEmpty(authHeader) && AuthenticationHeaderValue.TryParse(authHeader, out var parsed))
            client.DefaultRequestHeaders.Authorization = parsed;

        return client;
    }

    private static async Task<List<RemoteCheckInDto>> GetCheckInsAsync(
        HttpClient client, Guid? employeeId, DateTime from, DateTime to, CancellationToken ct)
    {
        var all = new List<RemoteCheckInDto>();
        for (var page = 1; page <= MaxCheckInPages; page++)
        {
            var url = $"/api/v1/checkins?from={Uri.EscapeDataString(from.ToString("o"))}" +
                      $"&to={Uri.EscapeDataString(to.ToString("o"))}" +
                      $"&page={page}&size={CheckInPageSize}" +
                      (employeeId.HasValue ? $"&employeeId={employeeId.Value}" : string.Empty);

            var response = await client.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode) break;

            var paged = await response.Content
                .ReadFromJsonAsync<RemotePagedResult<RemoteCheckInDto>>(JsonOptions, ct);
            if (paged is null || paged.Items.Count == 0) break;

            all.AddRange(paged.Items);
            if (all.Count >= paged.TotalCount || paged.Items.Count < CheckInPageSize) break;
        }

        return all;
    }

    private static async Task<Dictionary<Guid, string>> GetShiftNamesAsync(HttpClient client, CancellationToken ct)
    {
        var response = await client.GetAsync("/api/v1/shifts", ct);
        if (!response.IsSuccessStatusCode) return new Dictionary<Guid, string>();

        var shifts = await response.Content
            .ReadFromJsonAsync<List<RemoteShiftDto>>(JsonOptions, ct);
        return shifts?.ToDictionary(s => s.Id, s => s.Name) ?? new Dictionary<Guid, string>();
    }

    private static async Task<Dictionary<Guid, RemoteEmployeeDto>> GetEmployeeLookupAsync(
        HttpClient client, CancellationToken ct)
    {
        // V1 simplification: one large page rather than a proper bulk-by-ids lookup or
        // full pagination. Fine for a single-tenant dev/demo workforce; revisit if a tenant's
        // headcount ever approaches this page size.
        var response = await client.GetAsync("/api/v1/employees?page=1&size=5000", ct);
        if (!response.IsSuccessStatusCode) return new Dictionary<Guid, RemoteEmployeeDto>();

        var envelope = await response.Content
            .ReadFromJsonAsync<RemoteApiResponse<RemotePagedResult<RemoteEmployeeDto>>>(JsonOptions, ct);
        var items = envelope?.Data?.Items ?? new List<RemoteEmployeeDto>();
        return items.ToDictionary(e => e.Id);
    }

    private record RemoteCheckInDto(Guid EmployeeId, Guid ShiftId, DateTime CheckInTime, DateTime? CheckOutTime, bool IsWithinGeofence);
    private record RemoteShiftDto(Guid Id, string Name);
    private record RemoteEmployeeDto(Guid Id, string FullNameEn, string FullNameAr, string Iqama);
    private record RemotePagedResult<T>(List<T> Items, int TotalCount, int Page, int PageSize);
    private record RemoteApiResponse<T>(T? Data, object? Error, string TraceId);
}
