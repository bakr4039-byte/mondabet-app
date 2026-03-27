using MediatR;
using Mondabet.Clarification.Application.DTOs;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;

namespace Mondabet.Clarification.Application.Queries.ListClarifications;

public record ListClarificationsQuery(Guid EmployeeId)
    : IRequest<Result<IReadOnlyList<ClarificationDto>>>;
