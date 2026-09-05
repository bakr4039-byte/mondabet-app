using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Mondabet.Attendance.Application.Interfaces;

namespace Mondabet.Attendance.Infrastructure.Services;

/// <summary>
/// Same named-HttpClientFactory + bearer-forwarding pattern already established by
/// Report's PayrollDataService/AttendanceDataService for cross-microservice reads:
/// the caller's own Authorization header is forwarded so the Employee service sees
/// the original employee's token rather than a service-to-service credential.
/// </summary>
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

    public async Task<Guid?> GetShiftIdAsync(Guid employeeId, CancellationToken ct = default)
    {
        var client = CreateAuthorizedClient("EmployeeApi");
        var response = await client.GetAsync($"/api/v1/employees/{employeeId}", ct);
        if (!response.IsSuccessStatusCode) return null;

        var envelope = await response.Content
            .ReadFromJsonAsync<RemoteApiResponse<RemoteEmployeeDto>>(JsonOptions, ct);
        return envelope?.Data?.ShiftId;
    }

    private HttpClient CreateAuthorizedClient(string name)
    {
        var client = _httpClientFactory.CreateClient(name);
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrEmpty(authHeader) && AuthenticationHeaderValue.TryParse(authHeader, out var parsed))
            client.DefaultRequestHeaders.Authorization = parsed;
        return client;
    }

    private record RemoteEmployeeDto(Guid Id, Guid? ShiftId);
    private record RemoteApiResponse<T>(T? Data, object? Error, string TraceId);
}
