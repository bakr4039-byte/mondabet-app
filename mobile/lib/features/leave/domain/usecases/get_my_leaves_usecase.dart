import 'package:dartz/dartz.dart';

import '../../../../core/error/failures.dart';
import '../entities/leave_request.dart';
import '../repositories/leave_repository.dart';

class GetMyLeavesUseCase {
  final LeaveRepository repository;
  GetMyLeavesUseCase(this.repository);

  Future<Either<Failure, List<LeaveRequest>>> call({
    LeaveType? type,
    LeaveStatus? status,
  }) =>
      repository.getMyLeaves(type: type, status: status);
}
