using MediatR;
using Mondabet.Leave.Application.DTOs;
using Mondabet.Leave.Application.Interfaces;
using Mondabet.Leave.Domain.Entities;
using Mondabet.Shared.Domain;

namespace Mondabet.Leave.Application.Queries.GetSubstituteCandidates;

/// Suggests available substitute colleagues for an approved leave/permission/excuse
/// request: same department + same job title (subject/specialty) as the employee who's
/// out, excluding anyone who themselves has an overlapping approved leave. Idea from
/// researching teacher/school substitute-management software (TCP Software's absence
/// management feature set - credential-matched automatic substitute suggestion) - the
/// existing Leave flow only approved/rejected with no coverage suggestion at all.
public class GetSubstituteCandidatesQueryHandler
    : IRequestHandler<GetSubstituteCandidatesQuery, Result<IReadOnlyList<SubstituteCandidateDto>>>
{
    private const int MaxCandidates = 5;
    private const int OverlapCheckPageSize = 200;

    private readonly ILeaveRepository _leaveRepo;
    private readonly IEmployeeLookupService _employeeLookup;

    public GetSubstituteCandidatesQueryHandler(ILeaveRepository leaveRepo, IEmployeeLookupService employeeLookup)
    {
        _leaveRepo = leaveRepo;
        _employeeLookup = employeeLookup;
    }

    public async Task<Result<IReadOnlyList<SubstituteCandidateDto>>> Handle(
        GetSubstituteCandidatesQuery request, CancellationToken ct)
    {
        var leave = await _leaveRepo.GetByIdAsync(request.LeaveId, ct);
        if (leave is null) return Error.NotFound("LeaveRequest", request.LeaveId);
        if (leave.Status != LeaveStatus.Approved)
            return Error.Validation("Substitute suggestions are only available for approved requests.");

        var requester = await _employeeLookup.GetByIdAsync(leave.EmployeeId, ct);
        if (requester is null) return Error.NotFound("Employee", leave.EmployeeId);

        var rangeStart = leave.StartDate;
        var rangeEnd = leave.EndDate ?? leave.StartDate;

        var colleagues = await _employeeLookup.GetByDepartmentAsync(requester.DepartmentId, ct);

        var candidates = new List<SubstituteCandidateDto>();
        foreach (var candidate in colleagues)
        {
            if (candidates.Count >= MaxCandidates) break;
            if (candidate.Id == requester.Id || !candidate.IsActive) continue;
            if (!string.Equals(candidate.JobTitle, requester.JobTitle, StringComparison.OrdinalIgnoreCase))
                continue;

            var (existingApproved, _) = await _leaveRepo.GetPagedAsync(
                candidate.Id, null, LeaveStatus.Approved, 1, OverlapCheckPageSize, ct);
            var alreadyOut = existingApproved.Any(l =>
                l.StartDate <= rangeEnd && rangeStart <= (l.EndDate ?? l.StartDate));
            if (alreadyOut) continue;

            candidates.Add(new SubstituteCandidateDto(
                candidate.Id, candidate.FullNameAr, candidate.FullNameEn, candidate.JobTitle));
        }

        return candidates;
    }
}
