using MediatR;
using Mondabet.Employee.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Employee.Application.Commands.UpdateDepartment;

public record UpdateDepartmentCommand(Guid Id, DepartmentUpdateDto Dto) : IRequest<Result<DepartmentDto>>;
