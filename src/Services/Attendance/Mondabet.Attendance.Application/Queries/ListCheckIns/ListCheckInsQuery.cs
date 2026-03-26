using MediatR;
using Mondabet.Attendance.Application.DTOs;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;

namespace Mondabet.Attendance.Application.Queries.ListCheckIns;

public record ListCheckInsQuery(
    Guid? EmployeeId,
    DateTime? From,
    DateTime? To,
    int Page = 1,
    int Size = 20) : IRequest<Result<PagedResult<CheckInDto>>>;
