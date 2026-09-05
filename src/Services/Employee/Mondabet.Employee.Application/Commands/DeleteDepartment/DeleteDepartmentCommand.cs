using MediatR;
using Mondabet.Shared.Domain;

namespace Mondabet.Employee.Application.Commands.DeleteDepartment;

public record DeleteDepartmentCommand(Guid Id) : IRequest<Result<bool>>;
