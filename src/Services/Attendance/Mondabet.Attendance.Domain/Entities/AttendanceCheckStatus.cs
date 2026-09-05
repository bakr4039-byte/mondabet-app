namespace Mondabet.Attendance.Domain.Entities;

/// <summary>
/// Outcome of a single check-in/check-out attempt. Mirrors the status vocabulary
/// used across reporting and the portals so a single enum drives both enforcement
/// and display (on_time / late / out_of_fence_rejected / ...).
/// </summary>
public enum AttendanceCheckStatus
{
    OnTime = 0,
    Late = 1,
    OutOfFenceRejected = 2,
    OutOfShiftRejected = 3,
    EarlyDeparture = 4,
    Normal = 5,
    FieldMission = 6,
    NotScheduledToday = 7,
}
