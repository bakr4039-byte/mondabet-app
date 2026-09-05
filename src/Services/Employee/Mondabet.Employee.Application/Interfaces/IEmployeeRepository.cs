using EmployeeEntity = Mondabet.Employee.Domain.Entities.Employee;
using Mondabet.Shared.Application;

namespace Mondabet.Employee.Application.Interfaces;

public interface IEmployeeRepository : IRepository<EmployeeEntity>
{
    Task<EmployeeEntity?> GetByIqamaAsync(string iqama, CancellationToken ct = default);
    Task<bool> IqamaExistsAsync(string iqama, CancellationToken ct = default);
    Task<(IReadOnlyList<EmployeeEntity> Items, int Total)> GetPagedAsync(
        int page, int size, string? search, CancellationToken ct = default);

    /// <summary>Number of (non-deleted) employees currently assigned to a department.</summary>
    Task<int> CountByDepartmentAsync(Guid departmentId, CancellationToken ct = default);
}
