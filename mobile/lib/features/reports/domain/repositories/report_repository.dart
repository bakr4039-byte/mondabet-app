import 'package:dartz/dartz.dart';

import '../../../../core/error/failures.dart';

class AttendanceSummaryEntry {
  final String date;
  final int present;
  final int absent;
  const AttendanceSummaryEntry({required this.date, required this.present, required this.absent});
}

abstract class ReportRepository {
  Future<Either<Failure, List<AttendanceSummaryEntry>>> getAttendanceSummary({
    required DateTime from,
    required DateTime to,
  });
  Future<Either<Failure, String>> downloadReport({
    required String format,
    required DateTime from,
    required DateTime to,
  });
}
