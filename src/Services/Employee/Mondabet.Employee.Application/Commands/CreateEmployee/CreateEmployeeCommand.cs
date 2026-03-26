using MediatR;
using Mondabet.Employee.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Employee.Application.Commands.CreateEmployee;

public record CreateEmployeeCommand(EmployeeCreateDto Dto) : IRequest<Result<EmployeeDto>>;
