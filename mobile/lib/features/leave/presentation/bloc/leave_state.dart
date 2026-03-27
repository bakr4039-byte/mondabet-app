import 'package:equatable/equatable.dart';

import '../../../../core/error/failures.dart';
import '../../domain/entities/leave_request.dart';

abstract class LeaveState extends Equatable {
  const LeaveState();
  @override
  List<Object?> get props => [];
}

class LeaveInitial extends LeaveState {
  const LeaveInitial();
}

class LeaveLoading extends LeaveState {
  const LeaveLoading();
}

class LeaveListLoaded extends LeaveState {
  final List<LeaveRequest> leaves;
  const LeaveListLoaded(this.leaves);
  @override
  List<Object?> get props => [leaves];
}

class LeaveSubmitSuccess extends LeaveState {
  final LeaveRequest request;
  const LeaveSubmitSuccess(this.request);
  @override
  List<Object?> get props => [request];
}

class LeaveFailure extends LeaveState {
  final Failure failure;
  const LeaveFailure(this.failure);
  @override
  List<Object?> get props => [failure];
}
