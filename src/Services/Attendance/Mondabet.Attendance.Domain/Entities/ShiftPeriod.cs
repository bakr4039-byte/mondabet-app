namespace Mondabet.Attendance.Domain.Entities;

/// <summary>
/// Which half of a split shift (see <see cref="Shift.IsSplitShift"/>) an
/// <see cref="AttendanceRecord"/> belongs to. Null/unset for a normal (non-split) shift.
/// </summary>
public enum ShiftPeriod
{
    First = 0,
    Second = 1,
}
