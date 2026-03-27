import 'package:dartz/dartz.dart';

import '../../../../core/error/failures.dart';
import '../entities/leave_request.dart';
import '../repositories/leave_repository.dart';

class SubmitLeaveUseCase {
  final LeaveRepository repository;
  SubmitLeaveUseCase(this.repository);

  Future<Either<Failure, LeaveRequest>> call({
    required LeaveType type,
    required DateTime fromDate,
    required DateTime toDate,
    String? reason,
    String? filePath,
  }) =>
      repository.submitLeave(
        type: type,
        fromDate: fromDate,
        toDate: toDate,
        reason: reason,
        filePath: filePath,
      );
}
