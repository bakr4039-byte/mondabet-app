using Mondabet.Leave.Domain.Entities;
using Mondabet.Shared.Application;

namespace Mondabet.Leave.Application.Interfaces;

public interface ILeaveRepository : IRepository<LeaveRequest>
{
    Task<(IReadOnlyList<LeaveRequest> Items, int Total)> GetPagedAsync(
        Guid? employeeId, LeaveType? type, LeaveStatus? status,
        int page, int size, CancellationToken ct = default);
}
