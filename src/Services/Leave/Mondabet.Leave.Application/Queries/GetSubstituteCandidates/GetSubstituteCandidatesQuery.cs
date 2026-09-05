using MediatR;
using Mondabet.Leave.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Leave.Application.Queries.GetSubstituteCandidates;

public record GetSubstituteCandidatesQuery(Guid LeaveId)
    : IRequest<Result<IReadOnlyList<SubstituteCandidateDto>>>;
