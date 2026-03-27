import 'package:dio/dio.dart';

import '../models/attendance_model.dart';
import '../models/shift_model.dart';

abstract class AttendanceRemoteDataSource {
  Future<ShiftModel?> getCurrentShift();
  Future<AttendanceModel> checkIn({
    required String shiftId,
    required double lat,
    required double lng,
    required String deviceId,
  });
  Future<AttendanceModel> checkOut(String recordId);
  Future<List<AttendanceModel>> getMyAttendance({DateTime? from, DateTime? to});
}

class AttendanceRemoteDataSourceImpl implements AttendanceRemoteDataSource {
  final Dio _dio;
  AttendanceRemoteDataSourceImpl(this._dio);

  @override
  Future<ShiftModel?> getCurrentShift() async {
    final resp = await _dio.get<Map<String, dynamic>>('/shifts/current');
    if (resp.data == null) return null;
    return ShiftModel.fromJson(resp.data!);
  }

  @override
  Future<AttendanceModel> checkIn({
    required String shiftId,
    required double lat,
    required double lng,
    required String deviceId,
  }) async {
    final resp = await _dio.post<Map<String, dynamic>>(
      '/checkins',
      data: {'shiftId': shiftId, 'lat': lat, 'lng': lng, 'deviceId': deviceId},
    );
    return AttendanceModel.fromJson(resp.data!);
  }

  @override
  Future<AttendanceModel> checkOut(String recordId) async {
    final resp = await _dio.post<Map<String, dynamic>>('/checkins/$recordId/checkout');
    return AttendanceModel.fromJson(resp.data!);
  }

  @override
  Future<List<AttendanceModel>> getMyAttendance({DateTime? from, DateTime? to}) async {
    final resp = await _dio.get<Map<String, dynamic>>(
      '/checkins/my',
      queryParameters: {
        if (from != null) 'from': from.toIso8601String().split('T')[0],
        if (to != null) 'to': to.toIso8601String().split('T')[0],
      },
    );
    final items = (resp.data!['items'] as List)
        .map((e) => AttendanceModel.fromJson(e as Map<String, dynamic>))
        .toList();
    return items;
  }
}
