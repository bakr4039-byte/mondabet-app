using MediatR;
using Mondabet.Employee.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Employee.Application.Commands.CreateDepartment;

public record CreateDepartmentCommand(DepartmentCreateDto Dto) : IRequest<Result<DepartmentDto>>;
