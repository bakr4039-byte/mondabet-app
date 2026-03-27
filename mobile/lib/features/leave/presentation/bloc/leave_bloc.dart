import 'package:flutter_bloc/flutter_bloc.dart';

import '../../domain/usecases/get_my_leaves_usecase.dart';
import '../../domain/usecases/submit_leave_usecase.dart';
import 'leave_event.dart';
import 'leave_state.dart';

class LeaveBloc extends Bloc<LeaveEvent, LeaveState> {
  final SubmitLeaveUseCase submitLeave;
  final GetMyLeavesUseCase getMyLeaves;

  LeaveBloc({required this.submitLeave, required this.getMyLeaves})
      : super(const LeaveInitial()) {
    on<LeavesLoaded>(_onLoaded);
    on<LeaveSubmitted>(_onSubmit);
  }

  Future<void> _onLoaded(LeavesLoaded event, Emitter<LeaveState> emit) async {
    emit(const LeaveLoading());
    final result = await getMyLeaves(
      type: event.filterType,
      status: event.filterStatus,
    );
    result.fold(
      (f) => emit(LeaveFailure(f)),
      (leaves) => emit(LeaveListLoaded(leaves)),
    );
  }

  Future<void> _onSubmit(LeaveSubmitted event, Emitter<LeaveState> emit) async {
    emit(const LeaveLoading());
    final result = await submitLeave(
      type: event.type,
      fromDate: event.fromDate,
      toDate: event.toDate,
      reason: event.reason,
      filePath: event.filePath,
    );
    result.fold(
      (f) => emit(LeaveFailure(f)),
      (req) => emit(LeaveSubmitSuccess(req)),
    );
  }
}
