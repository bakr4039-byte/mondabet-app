import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';

import '../../../../core/error/failures.dart';
import '../../domain/repositories/report_repository.dart';
import '../datasources/report_remote_datasource.dart';

class ReportRepositoryImpl implements ReportRepository {
  final ReportRemoteDataSource remoteDataSource;
  ReportRepositoryImpl(this.remoteDataSource);

  @override
  Future<Either<Failure, List<AttendanceSummaryEntry>>> getAttendanceSummary({
    required DateTime from,
    required DateTime to,
  }) async {
    try {
      final result = await remoteDataSource.getAttendanceSummary(from: from, to: to);
      return Right(result);
    } on DioException catch (e) {
      return Left(ServerFailure(e.response?.statusCode?.toString() ?? 'unknown', e.message ?? 'Network error'));
    }
  }

  @override
  Future<Either<Failure, String>> downloadReport({
    required String format,
    required DateTime from,
    required DateTime to,
  }) async {
    try {
      final path = await remoteDataSource.downloadReport(format: format, from: from, to: to);
      return Right(path);
    } on DioException catch (e) {
      return Left(ServerFailure(e.response?.statusCode?.toString() ?? 'unknown', e.message ?? 'Network error'));
    }
  }
}
