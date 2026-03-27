import '../../domain/entities/attendance_record.dart';

class AttendanceModel extends AttendanceRecord {
  const AttendanceModel({
    required super.id,
    required super.employeeId,
    required super.shiftId,
    required super.checkInTime,
    super.checkOutTime,
    required super.checkInLat,
    required super.checkInLng,
    required super.isWithinGeofence,
  });

  factory AttendanceModel.fromJson(Map<String, dynamic> json) => AttendanceModel(
        id: json['id'] as String,
        employeeId: json['employeeId'] as String,
        shiftId: json['shiftId'] as String,
        checkInTime: DateTime.parse(json['checkInTime'] as String),
        checkOutTime: json['checkOutTime'] != null
            ? DateTime.parse(json['checkOutTime'] as String)
            : null,
        checkInLat: (json['checkInLat'] as num).toDouble(),
        checkInLng: (json['checkInLng'] as num).toDouble(),
        isWithinGeofence: json['isWithinGeofence'] as bool,
      );
}
