using MediatR;
using Mondabet.Employee.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Employee.Application.Queries.GetEmployee;

public record GetEmployeeQuery(Guid EmployeeId) : IRequest<Result<EmployeeDto>>;
