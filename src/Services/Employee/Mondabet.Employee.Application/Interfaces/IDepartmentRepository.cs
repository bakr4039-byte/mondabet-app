using Mondabet.Employee.Domain.Entities;
using Mondabet.Shared.Application;

namespace Mondabet.Employee.Application.Interfaces;

public interface IDepartmentRepository : IRepository<Department>
{
    Task<bool> CodeExistsAsync(string code, Guid? excludeId = null, CancellationToken ct = default);
}
