using Mondabet.Leave.Domain.Events;
using Mondabet.Shared.Domain;

namespace Mondabet.Leave.Domain.Entities;

public enum LeaveType : byte { Vacation = 1, Permission = 2, Excuse = 3 }
public enum LeaveStatus : byte { Pending = 1, Approved = 2, Rejected = 3 }

public class LeaveRequest : BaseEntity
{
    public Guid EmployeeId { get; private set; }
    public LeaveType LeaveType { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }         // vacations only
    public TimeOnly? StartTime { get; private set; }       // permissions only
    public TimeOnly? EndTime { get; private set; }         // permissions only
    public string Reason { get; private set; } = default!;
    public LeaveStatus Status { get; private set; }
    public string? RejectionReason { get; private set; }
    public string? ApprovalComment { get; private set; }

    private LeaveRequest() { }

    public static LeaveRequest Create(
        Guid employeeId, LeaveType type, DateOnly startDate,
        string reason,
        DateOnly? endDate = null,
        TimeOnly? startTime = null,
        TimeOnly? endTime = null) => new()
    {
        Id = Guid.NewGuid(),
        EmployeeId = employeeId,
        LeaveType = type,
        StartDate = startDate,
        EndDate = endDate,
        StartTime = startTime,
        EndTime = endTime,
        Reason = reason,
        Status = LeaveStatus.Pending,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
    };

    public void Approve(string? comment)
    {
        Status = LeaveStatus.Approved;
        ApprovalComment = comment;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new LeaveApprovedEvent(Id, EmployeeId));
    }

    public void Reject(string reason)
    {
        Status = LeaveStatus.Rejected;
        RejectionReason = reason;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new LeaveRejectedEvent(Id, EmployeeId));
    }
}
