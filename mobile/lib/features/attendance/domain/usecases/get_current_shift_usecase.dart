import 'package:dartz/dartz.dart';

import '../../../../core/error/failures.dart';
import '../entities/shift.dart';
import '../repositories/attendance_repository.dart';

class GetCurrentShiftUseCase {
  final AttendanceRepository repository;
  GetCurrentShiftUseCase(this.repository);

  Future<Either<Failure, Shift?>> call() => repository.getCurrentShift();
}
