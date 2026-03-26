using MediatR;
using Mondabet.Employee.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Employee.Application.Commands.ImportEmployees;

public record ImportEmployeesCommand(Stream ExcelStream) : IRequest<Result<ImportResultDto>>;
