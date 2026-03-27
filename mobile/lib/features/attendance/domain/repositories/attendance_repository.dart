import 'package:dartz/dartz.dart';

import '../../../../core/error/failures.dart';
import '../entities/attendance_record.dart';
import '../entities/shift.dart';

abstract class AttendanceRepository {
  Future<Either<Failure, Shift?>> getCurrentShift();
  Future<Either<Failure, AttendanceRecord>> checkIn({
    required String shiftId,
    required double lat,
    required double lng,
    required String deviceId,
  });
  Future<Either<Failure, AttendanceRecord>> checkOut(String recordId);
  Future<Either<Failure, List<AttendanceRecord>>> getMyAttendance({
    DateTime? from,
    DateTime? to,
  });
}
