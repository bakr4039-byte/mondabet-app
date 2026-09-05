namespace Mondabet.Attendance.Application.Interfaces;

/// <summary>
/// Cross-service lookup of an employee's assigned shift, owned by the Employee service.
/// Backs GET /api/v1/shifts/current so a normal employee token (not just CompanyAdmin)
/// can resolve their own shift without calling the CompanyAdmin-only /shifts list.
/// </summary>
public interface IEmployeeLookupService
{
    Task<Guid?> GetShiftIdAsync(Guid employeeId, CancellationToken ct = default);
}
