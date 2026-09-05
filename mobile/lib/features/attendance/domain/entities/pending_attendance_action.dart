enum PendingActionType { checkIn, checkOut }

/// A check-in or check-out the employee performed while offline, queued locally
/// until connectivity returns. See AttendanceLocalDataSource for the Hive-backed
/// storage and AttendanceRepositoryImpl for how these get flushed.
class PendingAttendanceAction {
  final String id;
  final PendingActionType type;
  final DateTime queuedAt;
  final String? shiftId;
  final double lat;
  final double lng;
  final String? deviceId;

  const PendingAttendanceAction({
    required this.id,
    required this.type,
    required this.queuedAt,
    this.shiftId,
    required this.lat,
    required this.lng,
    this.deviceId,
  });
}
