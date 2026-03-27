using MediatR;
using Mondabet.Clarification.Application.DTOs;
using Mondabet.Clarification.Application.Interfaces;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;
using ClarificationEntity = Mondabet.Clarification.Domain.Entities.Clarification;

namespace Mondabet.Clarification.Application.Commands.CreateClarification;

public class CreateClarificationCommandHandler
    : IRequestHandler<CreateClarificationCommand, Result<ClarificationDto>>
{
    private readonly IClarificationRepository _repo;
    private readonly IUnitOfWork _uow;

    public CreateClarificationCommandHandler(IClarificationRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<ClarificationDto>> Handle(
        CreateClarificationCommand request, CancellationToken ct)
    {
        var d = request.Dto;
        var clar = ClarificationEntity.Create(d.EmployeeId, d.FromDate, d.ToDate, d.Question);
        await _repo.AddAsync(clar, ct);
        await _uow.SaveChangesAsync(ct);
        return ToDto(clar);
    }

    internal static ClarificationDto ToDto(ClarificationEntity c) => new(
        c.Id, c.EmployeeId, c.FromDate, c.ToDate,
        c.Question, c.ResponseText, c.Status, c.CreatedAt);
}
