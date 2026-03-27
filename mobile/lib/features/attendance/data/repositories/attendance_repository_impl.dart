import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';

import '../../../../core/error/failures.dart';
import '../../domain/entities/attendance_record.dart';
import '../../domain/entities/shift.dart';
import '../../domain/repositories/attendance_repository.dart';
import '../datasources/attendance_remote_datasource.dart';

class AttendanceRepositoryImpl implements AttendanceRepository {
  final AttendanceRemoteDataSource remoteDataSource;
  AttendanceRepositoryImpl(this.remoteDataSource);

  @override
  Future<Either<Failure, Shift?>> getCurrentShift() async {
    try {
      final shift = await remoteDataSource.getCurrentShift();
      return Right(shift);
    } on DioException catch (e) {
      return Left(ServerFailure(e.message ?? 'Network error'));
    }
  }

  @override
  Future<Either<Failure, AttendanceRecord>> checkIn({
    required String shiftId,
    required double lat,
    required double lng,
    required String deviceId,
  }) async {
    try {
      final record = await remoteDataSource.checkIn(
        shiftId: shiftId, lat: lat, lng: lng, deviceId: deviceId,
      );
      return Right(record);
    } on DioException catch (e) {
      return Left(ServerFailure(e.message ?? 'Network error'));
    }
  }

  @override
  Future<Either<Failure, AttendanceRecord>> checkOut(String recordId) async {
    try {
      final record = await remoteDataSource.checkOut(recordId);
      return Right(record);
    } on DioException catch (e) {
      return Left(ServerFailure(e.message ?? 'Network error'));
    }
  }

  @override
  Future<Either<Failure, List<AttendanceRecord>>> getMyAttendance({
    DateTime? from,
    DateTime? to,
  }) async {
    try {
      final records = await remoteDataSource.getMyAttendance(from: from, to: to);
      return Right(records);
    } on DioException catch (e) {
      return Left(ServerFailure(e.message ?? 'Network error'));
    }
  }
}
