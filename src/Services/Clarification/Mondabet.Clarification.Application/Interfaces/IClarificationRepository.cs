using ClarificationEntity = Mondabet.Clarification.Domain.Entities.Clarification;
using Mondabet.Shared.Application;

namespace Mondabet.Clarification.Application.Interfaces;

public interface IClarificationRepository : IRepository<ClarificationEntity>
{
    Task<IReadOnlyList<ClarificationEntity>> GetByEmployeeAsync(
        Guid employeeId, CancellationToken ct = default);
}
