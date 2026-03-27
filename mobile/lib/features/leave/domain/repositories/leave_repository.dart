import 'package:dartz/dartz.dart';

import '../../../../core/error/failures.dart';
import '../entities/leave_request.dart';

abstract class LeaveRepository {
  Future<Either<Failure, LeaveRequest>> submitLeave({
    required LeaveType type,
    required DateTime fromDate,
    required DateTime toDate,
    String? reason,
    String? filePath,
  });

  Future<Either<Failure, List<LeaveRequest>>> getMyLeaves({
    LeaveType? type,
    LeaveStatus? status,
  });
}
