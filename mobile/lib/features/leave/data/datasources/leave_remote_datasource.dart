import 'dart:io';

import 'package:dio/dio.dart';

import '../models/leave_model.dart';
import '../../domain/entities/leave_request.dart';

abstract class LeaveRemoteDataSource {
  Future<LeaveModel> submitLeave({
    required LeaveType type,
    required DateTime fromDate,
    required DateTime toDate,
    String? reason,
    String? filePath,
  });
  Future<List<LeaveModel>> getMyLeaves({LeaveType? type, LeaveStatus? status});
}

class LeaveRemoteDataSourceImpl implements LeaveRemoteDataSource {
  final Dio _dio;
  LeaveRemoteDataSourceImpl(this._dio);

  @override
  Future<LeaveModel> submitLeave({
    required LeaveType type,
    required DateTime fromDate,
    required DateTime toDate,
    String? reason,
    String? filePath,
  }) async {
    final typeMap = {
      LeaveType.vacation: 1,
      LeaveType.permission: 2,
      LeaveType.excuse: 3,
    };

    FormData formData = FormData.fromMap({
      'employeeId': 'me',
      'type': typeMap[type],
      'fromDate': fromDate.toIso8601String().split('T')[0],
      'toDate': toDate.toIso8601String().split('T')[0],
      if (reason != null) 'reason': reason,
      if (filePath != null)
        'file': await MultipartFile.fromFile(filePath,
            filename: filePath.split(Platform.pathSeparator).last),
    });

    final resp = await _dio.post<Map<String, dynamic>>('/leaves', data: formData);
    return LeaveModel.fromJson(resp.data!);
  }

  @override
  Future<List<LeaveModel>> getMyLeaves({LeaveType? type, LeaveStatus? status}) async {
    final typeMap = {
      LeaveType.vacation: 1,
      LeaveType.permission: 2,
      LeaveType.excuse: 3,
    };
    final statusMap = {
      LeaveStatus.pending: 1,
      LeaveStatus.approved: 2,
      LeaveStatus.rejected: 3,
    };

    final resp = await _dio.get<Map<String, dynamic>>(
      '/leaves/my',
      queryParameters: {
        if (type != null) 'type': typeMap[type],
        if (status != null) 'status': statusMap[status],
      },
    );
    return (resp.data!['items'] as List)
        .map((e) => LeaveModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }
}
