import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';

import '../../../../core/error/failures.dart';
import '../../domain/entities/leave_request.dart';
import '../../domain/repositories/leave_repository.dart';
import '../datasources/leave_remote_datasource.dart';

class LeaveRepositoryImpl implements LeaveRepository {
  final LeaveRemoteDataSource remoteDataSource;
  LeaveRepositoryImpl(this.remoteDataSource);

  @override
  Future<Either<Failure, LeaveRequest>> submitLeave({
    required LeaveType type,
    required DateTime fromDate,
    required DateTime toDate,
    String? reason,
    String? filePath,
  }) async {
    try {
      final result = await remoteDataSource.submitLeave(
        type: type, fromDate: fromDate, toDate: toDate,
        reason: reason, filePath: filePath,
      );
      return Right(result);
    } on DioException catch (e) {
      return Left(ServerFailure(e.message ?? 'Network error'));
    }
  }

  @override
  Future<Either<Failure, List<LeaveRequest>>> getMyLeaves({
    LeaveType? type,
    LeaveStatus? status,
  }) async {
    try {
      final results = await remoteDataSource.getMyLeaves(type: type, status: status);
      return Right(results);
    } on DioException catch (e) {
      return Left(ServerFailure(e.message ?? 'Network error'));
    }
  }
}
