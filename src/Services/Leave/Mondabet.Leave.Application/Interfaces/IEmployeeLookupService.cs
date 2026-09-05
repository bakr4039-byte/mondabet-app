namespace Mondabet.Leave.Application.Interfaces;

public record EmployeeSummary(
    Guid Id,
    string FullNameAr,
    string FullNameEn,
    string JobTitle,
    Guid DepartmentId,
    bool IsActive);

/// Cross-service lookup of employee/colleague info, owned by the Employee service.
/// Backs the substitute-candidate suggestion feature: finding who else shares the
/// same department and job title (subject/specialty) as an employee going on leave.
public interface IEmployeeLookupService
{
    Task<EmployeeSummary?> GetByIdAsync(Guid employeeId, CancellationToken ct = default);
    Task<IReadOnlyList<EmployeeSummary>> GetByDepartmentAsync(Guid departmentId, CancellationToken ct = default);
}
