class AttendanceRecord {
  final String id;
  final String employeeId;
  final String shiftId;
  final DateTime checkInTime;
  final DateTime? checkOutTime;
  final double checkInLat;
  final double checkInLng;
  final bool isWithinGeofence;

  /// True for a record created locally while offline (queued in Hive, not yet
  /// confirmed by the server). Cleared once the offline sync flushes it.
  final bool pendingSync;

  const AttendanceRecord({
    required this.id,
    required this.employeeId,
    required this.shiftId,
    required this.checkInTime,
    this.checkOutTime,
    required this.checkInLat,
    required this.checkInLng,
    required this.isWithinGeofence,
    this.pendingSync = false,
  });

  bool get isOpen => checkOutTime == null;
}
