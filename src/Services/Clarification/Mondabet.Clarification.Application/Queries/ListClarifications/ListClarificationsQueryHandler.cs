using MediatR;
using Mondabet.Clarification.Application.Commands.CreateClarification;
using Mondabet.Clarification.Application.DTOs;
using Mondabet.Clarification.Application.Interfaces;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;

namespace Mondabet.Clarification.Application.Queries.ListClarifications;

public class ListClarificationsQueryHandler
    : IRequestHandler<ListClarificationsQuery, Result<IReadOnlyList<ClarificationDto>>>
{
    private readonly IClarificationRepository _repo;

    public ListClarificationsQueryHandler(IClarificationRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<IReadOnlyList<ClarificationDto>>> Handle(
        ListClarificationsQuery request, CancellationToken ct)
    {
        var items = await _repo.GetByEmployeeAsync(request.EmployeeId, ct);
        var dtos = items.Select(CreateClarificationCommandHandler.ToDto).ToList();
        return Result<IReadOnlyList<ClarificationDto>>.Success(dtos);
    }
}
