import 'package:equatable/equatable.dart';

import '../../../../core/error/failures.dart';
import '../../domain/repositories/report_repository.dart';

abstract class ReportsState extends Equatable {
  const ReportsState();
  @override
  List<Object?> get props => [];
}

class ReportsInitial extends ReportsState {
  const ReportsInitial();
}

class ReportsLoading extends ReportsState {
  const ReportsLoading();
}

class ReportsSummaryLoaded extends ReportsState {
  final List<AttendanceSummaryEntry> entries;
  const ReportsSummaryLoaded(this.entries);
  @override
  List<Object?> get props => [entries];
}

class ReportDownloaded extends ReportsState {
  final String filePath;
  const ReportDownloaded(this.filePath);
  @override
  List<Object?> get props => [filePath];
}

class ReportsFailure extends ReportsState {
  final Failure failure;
  const ReportsFailure(this.failure);
  @override
  List<Object?> get props => [failure];
}
