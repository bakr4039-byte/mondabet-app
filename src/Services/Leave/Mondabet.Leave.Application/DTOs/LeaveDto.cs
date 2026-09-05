using Mondabet.Leave.Domain.Entities;

namespace Mondabet.Leave.Application.DTOs;

public record LeaveDto(
    Guid Id,
    Guid EmployeeId,
    LeaveType LeaveType,
    DateOnly StartDate,
    DateOnly? EndDate,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    string Reason,
    LeaveStatus Status,
    string? RejectionReason,
    string? ApprovalComment,
    DateTime CreatedAt);

public record SubmitLeaveDto(
    LeaveType LeaveType,
    DateOnly StartDate,
    string Reason,
    DateOnly? EndDate = null,
    TimeOnly? StartTime = null,
    TimeOnly? EndTime = null);

public record ApproveLeaveDto(string? Comment);

public record RejectLeaveDto(string Reason);

/// A colleague suggested as coverage for an approved leave/permission/excuse - same
/// department and job title (subject/specialty), not themselves already on approved
/// leave during the same date range. See GetSubstituteCandidatesQueryHandler.
public record SubstituteCandidateDto(
    Guid EmployeeId,
    string FullNameAr,
    string FullNameEn,
    string JobTitle);
