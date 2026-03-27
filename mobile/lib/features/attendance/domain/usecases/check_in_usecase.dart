import 'package:dartz/dartz.dart';

import '../../../../core/error/failures.dart';
import '../entities/attendance_record.dart';
import '../repositories/attendance_repository.dart';

class CheckInUseCase {
  final AttendanceRepository repository;
  CheckInUseCase(this.repository);

  Future<Either<Failure, AttendanceRecord>> call({
    required String shiftId,
    required double lat,
    required double lng,
    required String deviceId,
  }) =>
      repository.checkIn(shiftId: shiftId, lat: lat, lng: lng, deviceId: deviceId);
}
