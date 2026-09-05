import 'package:dartz/dartz.dart';

import '../../../../core/error/failures.dart';
import '../repositories/attendance_repository.dart';

class SyncPendingAttendanceUseCase {
  final AttendanceRepository repository;
  SyncPendingAttendanceUseCase(this.repository);

  Future<Either<Failure, int>> call() => repository.syncPendingActions();
}
