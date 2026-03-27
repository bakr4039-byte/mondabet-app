import 'package:equatable/equatable.dart';

import '../../domain/entities/leave_request.dart';

abstract class LeaveEvent extends Equatable {
  const LeaveEvent();
  @override
  List<Object?> get props => [];
}

class LeavesLoaded extends LeaveEvent {
  final LeaveType? filterType;
  final LeaveStatus? filterStatus;
  const LeavesLoaded({this.filterType, this.filterStatus});
  @override
  List<Object?> get props => [filterType, filterStatus];
}

class LeaveSubmitted extends LeaveEvent {
  final LeaveType type;
  final DateTime fromDate;
  final DateTime toDate;
  final String? reason;
  final String? filePath;

  const LeaveSubmitted({
    required this.type,
    required this.fromDate,
    required this.toDate,
    this.reason,
    this.filePath,
  });

  @override
  List<Object?> get props => [type, fromDate, toDate, reason, filePath];
}
