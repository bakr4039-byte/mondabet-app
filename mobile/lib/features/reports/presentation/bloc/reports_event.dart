import 'package:equatable/equatable.dart';

abstract class ReportsEvent extends Equatable {
  const ReportsEvent();
  @override
  List<Object?> get props => [];
}

class ReportsSummaryRequested extends ReportsEvent {
  final DateTime from;
  final DateTime to;
  const ReportsSummaryRequested({required this.from, required this.to});
  @override
  List<Object?> get props => [from, to];
}

class ReportDownloadRequested extends ReportsEvent {
  final String format;
  final DateTime from;
  final DateTime to;
  const ReportDownloadRequested({required this.format, required this.from, required this.to});
  @override
  List<Object?> get props => [format, from, to];
}
