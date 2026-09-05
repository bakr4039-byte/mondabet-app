using MediatR;
using Mondabet.Employee.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Employee.Application.Queries.ListDepartments;

public record ListDepartmentsQuery : IRequest<Result<IReadOnlyList<DepartmentDto>>>;
