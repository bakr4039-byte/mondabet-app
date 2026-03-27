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
