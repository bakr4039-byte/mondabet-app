using MediatR;
using Mondabet.Clarification.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Clarification.Application.Commands.RespondClarification;

public record RespondClarificationCommand(Guid ClarificationId, RespondClarificationDto Dto)
    : IRequest<Result<ClarificationDto>>;
