using MediatR;
using Mondabet.Employee.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Employee.Application.Commands.UpdateEmployee;

public record UpdateEmployeeCommand(Guid EmployeeId, EmployeeUpdateDto Dto)
    : IRequest<Result<EmployeeDto>>;
