class AttendanceRecord {
  final String id;
  final String employeeId;
  final String shiftId;
  final DateTime checkInTime;
  final DateTime? checkOutTime;
  final double checkInLat;
  final double checkInLng;
  final bool isWithinGeofence;

  const AttendanceRecord({
    required this.id,
    required this.employeeId,
    required this.shiftId,
    required this.checkInTime,
    this.checkOutTime,
    required this.checkInLat,
    required this.checkInLng,
    required this.isWithinGeofence,
  });

  bool get isOpen => checkOutTime == null;
}
