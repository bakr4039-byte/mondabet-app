using MediatR;
using Mondabet.Clarification.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Clarification.Application.Commands.CreateClarification;

public record CreateClarificationCommand(CreateClarificationDto Dto)
    : IRequest<Result<ClarificationDto>>;
