using MediatR;
using Mondabet.Employee.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Employee.Application.Queries.GetDepartment;

public record GetDepartmentQuery(Guid Id) : IRequest<Result<DepartmentDto>>;
