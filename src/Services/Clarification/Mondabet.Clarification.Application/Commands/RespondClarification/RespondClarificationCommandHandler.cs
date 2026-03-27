using MediatR;
using Mondabet.Clarification.Application.Commands.CreateClarification;
using Mondabet.Clarification.Application.DTOs;
using Mondabet.Clarification.Application.Interfaces;
using Mondabet.Clarification.Domain.Entities;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;

namespace Mondabet.Clarification.Application.Commands.RespondClarification;

public class RespondClarificationCommandHandler
    : IRequestHandler<RespondClarificationCommand, Result<ClarificationDto>>
{
    private readonly IClarificationRepository _repo;
    private readonly IUnitOfWork _uow;

    public RespondClarificationCommandHandler(IClarificationRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<ClarificationDto>> Handle(
        RespondClarificationCommand request, CancellationToken ct)
    {
        var clar = await _repo.GetByIdAsync(request.ClarificationId, ct);
        if (clar is null) return Error.NotFound("Clarification", request.ClarificationId);
        if (clar.Status == ClarificationStatus.Responded)
            return Error.Conflict("Clarification already has a response.");

        clar.Respond(request.Dto.ResponseText);
        _repo.Update(clar);
        await _uow.SaveChangesAsync(ct);
        return CreateClarificationCommandHandler.ToDto(clar);
    }
}
