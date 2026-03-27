import 'dart:io';

import 'package:dio/dio.dart';
import 'package:path_provider/path_provider.dart';

import '../../domain/repositories/report_repository.dart';

abstract class ReportRemoteDataSource {
  Future<List<AttendanceSummaryEntry>> getAttendanceSummary({
    required DateTime from,
    required DateTime to,
  });
  Future<String> downloadReport({
    required String format,
    required DateTime from,
    required DateTime to,
  });
}

class ReportRemoteDataSourceImpl implements ReportRemoteDataSource {
  final Dio _dio;
  ReportRemoteDataSourceImpl(this._dio);

  @override
  Future<List<AttendanceSummaryEntry>> getAttendanceSummary({
    required DateTime from,
    required DateTime to,
  }) async {
    final resp = await _dio.get<Map<String, dynamic>>(
      '/reports/attendance/summary',
      queryParameters: {
        'from': from.toIso8601String().split('T')[0],
        'to': to.toIso8601String().split('T')[0],
      },
    );
    return (resp.data!['items'] as List)
        .map((e) => AttendanceSummaryEntry(
              date: e['date'] as String,
              present: e['present'] as int,
              absent: e['absent'] as int,
            ))
        .toList();
  }

  @override
  Future<String> downloadReport({
    required String format,
    required DateTime from,
    required DateTime to,
  }) async {
    final dir = await getTemporaryDirectory();
    final ext = format == 'pdf' ? 'pdf' : 'xlsx';
    final filePath = '${dir.path}/attendance_report.$ext';

    await _dio.download(
      '/reports/attendance',
      filePath,
      queryParameters: {
        'format': format,
        'from': from.toIso8601String().split('T')[0],
        'to': to.toIso8601String().split('T')[0],
      },
    );
    return filePath;
  }
}
