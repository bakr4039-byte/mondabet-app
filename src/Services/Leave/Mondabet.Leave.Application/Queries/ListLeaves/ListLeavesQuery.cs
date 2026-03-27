using MediatR;
using Mondabet.Leave.Application.DTOs;
using Mondabet.Leave.Domain.Entities;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;

namespace Mondabet.Leave.Application.Queries.ListLeaves;

public record ListLeavesQuery(
    Guid? EmployeeId,
    LeaveType? Type,
    LeaveStatus? Status,
    int Page = 1,
    int Size = 20) : IRequest<Result<PagedResult<LeaveDto>>>;
