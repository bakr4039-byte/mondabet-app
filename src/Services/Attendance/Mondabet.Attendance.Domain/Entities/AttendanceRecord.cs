using Mondabet.Shared.Domain;

namespace Mondabet.Attendance.Domain.Entities;

public class AttendanceRecord : BaseEntity
{
    public Guid EmployeeId { get; private set; }
    public Guid ShiftId { get; private set; }
    public DateTime CheckInTime { get; private set; }
    public DateTime? CheckOutTime { get; private set; }
    public double CheckInLat { get; private set; }
    public double CheckInLng { get; private set; }
    public bool IsWithinGeofence { get; private set; }
    public string DeviceId { get; private set; } = default!;

    private AttendanceRecord() { }

    public static AttendanceRecord Create(
        Guid employeeId, Guid shiftId,
        double lat, double lng,
        bool isWithinGeofence, string deviceId) => new()
    {
        Id = Guid.NewGuid(),
        EmployeeId = employeeId,
        ShiftId = shiftId,
        CheckInTime = DateTime.UtcNow,
        CheckInLat = lat,
        CheckInLng = lng,
        IsWithinGeofence = isWithinGeofence,
        DeviceId = deviceId,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
    };

    public void CheckOut()
    {
        CheckOutTime = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
