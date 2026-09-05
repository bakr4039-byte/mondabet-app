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
  Future<Either<Failure, AttendanceRecord>> checkOut({double? lat, double? lng});
  Future<Either<Failure, List<AttendanceRecord>>> getMyAttendance({
    DateTime? from,
    DateTime? to,
  });

  /// Number of check-in/check-out actions queued locally while offline and
  /// not yet confirmed by the server.
  Future<int> getPendingCount();

  /// Attempts to push every queued offline action to the server, in the order
  /// they were performed. Returns how many were successfully synced. A no-op
  /// (returns 0) when there is no connection.
  Future<Either<Failure, int>> syncPendingActions();
}
