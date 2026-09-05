using Mondabet.Report.Application.Interfaces;

namespace Mondabet.Report.Infrastructure.Services;

/// <summary>
/// Stub data service — in production this queries tenant-svc's API.
/// Replace with a proper cross-service data fetch (HTTP client), same as
/// AttendanceDataService.cs did for attendance/employee data.
/// </summary>
public class StubTenantDataService : ITenantDataService
{
    public Task<IReadOnlyList<TenantSummaryRow>> GetTenantSummaryRowsAsync(
        CancellationToken ct = default)
    {
        IReadOnlyList<TenantSummaryRow> rows = Array.Empty<TenantSummaryRow>();
        return Task.FromResult(rows);
    }
}
