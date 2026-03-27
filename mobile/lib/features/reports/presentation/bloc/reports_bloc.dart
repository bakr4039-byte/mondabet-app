import 'package:flutter_bloc/flutter_bloc.dart';

import '../../domain/repositories/report_repository.dart';
import 'reports_event.dart';
import 'reports_state.dart';

class ReportsBloc extends Bloc<ReportsEvent, ReportsState> {
  final ReportRepository repository;

  ReportsBloc(this.repository) : super(const ReportsInitial()) {
    on<ReportsSummaryRequested>(_onSummary);
    on<ReportDownloadRequested>(_onDownload);
  }

  Future<void> _onSummary(
    ReportsSummaryRequested event,
    Emitter<ReportsState> emit,
  ) async {
    emit(const ReportsLoading());
    final result = await repository.getAttendanceSummary(from: event.from, to: event.to);
    result.fold(
      (f) => emit(ReportsFailure(f)),
      (entries) => emit(ReportsSummaryLoaded(entries)),
    );
  }

  Future<void> _onDownload(
    ReportDownloadRequested event,
    Emitter<ReportsState> emit,
  ) async {
    emit(const ReportsLoading());
    final result = await repository.downloadReport(
      format: event.format, from: event.from, to: event.to,
    );
    result.fold(
      (f) => emit(ReportsFailure(f)),
      (path) => emit(ReportDownloaded(path)),
    );
  }
}
