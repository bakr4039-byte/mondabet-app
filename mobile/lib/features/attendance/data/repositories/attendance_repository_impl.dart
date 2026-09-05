import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';

import '../../../../core/error/failures.dart';
import '../../../../core/network/connectivity_service.dart';
import '../../domain/entities/attendance_record.dart';
import '../../domain/entities/pending_attendance_action.dart';
import '../../domain/entities/shift.dart';
import '../../domain/repositories/attendance_repository.dart';
import '../datasources/attendance_local_datasource.dart';
import '../datasources/attendance_remote_datasource.dart';
import '../models/pending_action_model.dart';
import '../models/shift_model.dart';

/// Offline-aware: check-in/check-out are queued locally (Hive, via
/// AttendanceLocalDataSource) instead of failing outright when there is no
/// connection, and the last-known shift is cached so the geofence/map still
/// works offline. AttendanceBloc calls syncPendingActions() whenever
/// connectivity returns to flush the queue in order.
class AttendanceRepositoryImpl implements AttendanceRepository {
  final AttendanceRemoteDataSource remoteDataSource;
  final AttendanceLocalDataSource localDataSource;
  final ConnectivityService connectivityService;

  AttendanceRepositoryImpl(
    this.remoteDataSource,
    this.localDataSource,
    this.connectivityService,
  );

  @override
  Future<Either<Failure, Shift?>> getCurrentShift() async {
    if (!await connectivityService.isOnline) {
      final cached = await localDataSource.getCachedShift();
      return cached != null ? Right(cached) : const Left(NetworkFailure());
    }
    try {
      final shift = await remoteDataSource.getCurrentShift();
      if (shift != null) await localDataSource.cacheShift(shift);
      return Right(shift);
    } on DioException catch (e) {
      if (_isConnectivityError(e)) {
        final cached = await localDataSource.getCachedShift();
        if (cached != null) return Right(cached);
      }
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
    if (!await connectivityService.isOnline) {
      return Right(await _queueCheckIn(shiftId: shiftId, lat: lat, lng: lng, deviceId: deviceId));
    }
    try {
      final record = await remoteDataSource.checkIn(
        shiftId: shiftId, lat: lat, lng: lng, deviceId: deviceId,
      );
      return Right(record);
    } on DioException catch (e) {
      if (_isConnectivityError(e)) {
        return Right(await _queueCheckIn(shiftId: shiftId, lat: lat, lng: lng, deviceId: deviceId));
      }
      return Left(ServerFailure(e.message ?? 'Network error'));
    }
  }

  @override
  Future<Either<Failure, AttendanceRecord>> checkOut({double? lat, double? lng}) async {
    if (!await connectivityService.isOnline) {
      return Right(await _queueCheckOut(lat: lat, lng: lng));
    }
    try {
      final record = await remoteDataSource.checkOut(lat: lat, lng: lng);
      return Right(record);
    } on DioException catch (e) {
      if (_isConnectivityError(e)) {
        return Right(await _queueCheckOut(lat: lat, lng: lng));
      }
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

  @override
  Future<int> getPendingCount() async => (await localDataSource.getPendingActions()).length;

  @override
  Future<Either<Failure, int>> syncPendingActions() async {
    if (!await connectivityService.isOnline) return const Right(0);

    final pending = await localDataSource.getPendingActions();
    var synced = 0;

    for (final action in pending) {
      try {
        if (action.type == PendingActionType.checkIn) {
          await remoteDataSource.checkIn(
            shiftId: action.shiftId!,
            lat: action.lat,
            lng: action.lng,
            deviceId: action.deviceId ?? 'offline-sync',
          );
        } else {
          await remoteDataSource.checkOut(lat: action.lat, lng: action.lng);
        }
        await localDataSource.removePendingAction(action.id);
        synced++;
      } on DioException catch (e) {
        if (_isConnectivityError(e)) {
          // Lost the connection again mid-sync - stop and leave the rest
          // queued for the next attempt rather than failing them all.
          break;
        }
        // A real server-side rejection (e.g. "already checked in today").
        // Retrying it forever would just get the same error, so drop it -
        // the employee's actual state on the server is authoritative.
        await localDataSource.removePendingAction(action.id);
      }
    }

    return Right(synced);
  }

  Future<AttendanceRecord> _queueCheckIn({
    required String shiftId,
    required double lat,
    required double lng,
    required String deviceId,
  }) async {
    final now = DateTime.now();
    final action = PendingActionModel(
      id: 'pending-${now.microsecondsSinceEpoch}',
      type: PendingActionType.checkIn,
      queuedAt: now,
      shiftId: shiftId,
      lat: lat,
      lng: lng,
      deviceId: deviceId,
    );
    await localDataSource.enqueuePendingAction(action);

    return AttendanceRecord(
      id: action.id,
      employeeId: '',
      shiftId: shiftId,
      checkInTime: now,
      checkInLat: lat,
      checkInLng: lng,
      // Best-effort locally; the server re-validates the geofence for real
      // once this syncs, and rejects it there if it turns out to be outside.
      isWithinGeofence: true,
      pendingSync: true,
    );
  }

  Future<AttendanceRecord> _queueCheckOut({double? lat, double? lng}) async {
    final now = DateTime.now();
    final action = PendingActionModel(
      id: 'pending-${now.microsecondsSinceEpoch}',
      type: PendingActionType.checkOut,
      queuedAt: now,
      lat: lat ?? 0,
      lng: lng ?? 0,
    );
    await localDataSource.enqueuePendingAction(action);

    return AttendanceRecord(
      id: action.id,
      employeeId: '',
      shiftId: '',
      checkInTime: now,
      checkOutTime: now,
      checkInLat: lat ?? 0,
      checkInLng: lng ?? 0,
      isWithinGeofence: true,
      pendingSync: true,
    );
  }

  bool _isConnectivityError(DioException e) =>
      e.type == DioExceptionType.connectionError ||
      e.type == DioExceptionType.connectionTimeout ||
      e.type == DioExceptionType.receiveTimeout ||
      e.type == DioExceptionType.unknown;
}
