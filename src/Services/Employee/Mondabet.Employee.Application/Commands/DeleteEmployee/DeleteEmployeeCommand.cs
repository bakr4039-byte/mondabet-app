using MediatR;
using Mondabet.Shared.Domain;

namespace Mondabet.Employee.Application.Commands.DeleteEmployee;

public record DeleteEmployeeCommand(Guid EmployeeId) : IRequest<Result<bool>>;
