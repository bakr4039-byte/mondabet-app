using Mondabet.Report.Application.Interfaces;

namespace Mondabet.Report.Infrastructure.Services;

/// <summary>
/// Stub data service — in production this queries attendance-svc DB or calls its API.
/// Replace with a proper cross-service data fetch (HTTP client or shared read model).
/// </summary>
public class StubAttendanceDataService : IAttendanceDataService
{
    public Task<IReadOnlyList<AttendanceRow>> GetAttendanceRowsAsync(
        Guid? employeeId, DateTime from, DateTime to, CancellationToken ct = default)
    {
        IReadOnlyList<AttendanceRow> rows = Array.Empty<AttendanceRow>();
        return Task.FromResult(rows);
    }
}

public class StubTenantDataService : ITenantDataService
{
    public Task<IReadOnlyList<TenantSummaryRow>> GetTenantSummaryRowsAsync(
        CancellationToken ct = default)
    {
        IReadOnlyList<TenantSummaryRow> rows = Array.Empty<TenantSummaryRow>();
        return Task.FromResult(rows);
    }
}
