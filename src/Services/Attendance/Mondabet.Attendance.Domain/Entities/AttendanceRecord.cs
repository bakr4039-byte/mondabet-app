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
    public double? CheckOutLat { get; private set; }
    public double? CheckOutLng { get; private set; }
    public bool IsWithinGeofence { get; private set; }
    public string DeviceId { get; private set; } = default!;

    public AttendanceCheckStatus CheckInStatus { get; private set; } = AttendanceCheckStatus.Normal;
    public AttendanceCheckStatus? CheckOutStatus { get; private set; }
    public string? RejectionReason { get; private set; }

    // Biometric / anti-spoofing (idea carried over from the Google AI Studio prototype's
    // facial-verification + GPS-integrity concepts). All optional: a plain GPS check-in
    // still works with none of these set.
    public bool FacialVerified { get; private set; }
    public string? SelfiePhoto { get; private set; }
    public double? AntiSpoofingScore { get; private set; }

    // Field / kiosk punches bypass the office geofence by design.
    public bool IsFieldPunch { get; private set; }
    public string? FieldClientName { get; private set; }
    public bool IsKioskPunch { get; private set; }
    public string? KioskTerminalId { get; private set; }

    private AttendanceRecord() { }

    public static AttendanceRecord Create(
        Guid employeeId, Guid shiftId,
        double lat, double lng,
        bool isWithinGeofence, string deviceId,
        AttendanceCheckStatus checkInStatus = AttendanceCheckStatus.Normal,
        bool facialVerified = false,
        string? selfiePhoto = null,
        double? antiSpoofingScore = null,
        bool isFieldPunch = false,
        string? fieldClientName = null,
        bool isKioskPunch = false,
        string? kioskTerminalId = null) => new()
    {
        Id = Guid.NewGuid(),
        EmployeeId = employeeId,
        ShiftId = shiftId,
        CheckInTime = DateTime.UtcNow,
        CheckInLat = lat,
        CheckInLng = lng,
        IsWithinGeofence = isWithinGeofence,
        DeviceId = deviceId,
        CheckInStatus = checkInStatus,
        FacialVerified = facialVerified,
        SelfiePhoto = selfiePhoto,
        AntiSpoofingScore = antiSpoofingScore,
        IsFieldPunch = isFieldPunch,
        FieldClientName = fieldClientName,
        IsKioskPunch = isKioskPunch,
        KioskTerminalId = kioskTerminalId,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
    };

    public void CheckOut(
        double? lat = null, double? lng = null,
        AttendanceCheckStatus? status = null,
        string? note = null)
    {
        CheckOutTime = DateTime.UtcNow;
        CheckOutLat = lat;
        CheckOutLng = lng;
        CheckOutStatus = status;
        if (note is not null) RejectionReason = note;
        UpdatedAt = DateTime.UtcNow;
    }
}
