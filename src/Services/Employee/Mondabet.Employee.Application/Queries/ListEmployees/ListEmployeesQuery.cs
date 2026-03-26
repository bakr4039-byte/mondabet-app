using MediatR;
using Mondabet.Employee.Application.DTOs;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;

namespace Mondabet.Employee.Application.Queries.ListEmployees;

public record ListEmployeesQuery(int Page = 1, int Size = 20, string? Search = null)
    : IRequest<Result<PagedResult<EmployeeDto>>>;
