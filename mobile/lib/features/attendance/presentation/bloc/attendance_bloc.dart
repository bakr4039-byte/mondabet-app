import 'dart:math';

import 'package:flutter_bloc/flutter_bloc.dart';

import '../../domain/usecases/check_in_usecase.dart';
import '../../domain/usecases/check_out_usecase.dart';
import '../../domain/usecases/get_current_shift_usecase.dart';
import 'attendance_event.dart';
import 'attendance_state.dart';

class AttendanceBloc extends Bloc<AttendanceEvent, AttendanceState> {
  final GetCurrentShiftUseCase getCurrentShift;
  final CheckInUseCase checkIn;
  final CheckOutUseCase checkOut;

  AttendanceBloc({
    required this.getCurrentShift,
    required this.checkIn,
    required this.checkOut,
  }) : super(const AttendanceInitial()) {
    on<AttendanceStarted>(_onStarted);
    on<CheckInRequested>(_onCheckIn);
    on<CheckOutRequested>(_onCheckOut);
    on<LocationRefreshed>(_onLocationRefreshed);
  }

  Future<void> _onStarted(AttendanceStarted event, Emitter<AttendanceState> emit) async {
    emit(const AttendanceLoading());
    final result = await getCurrentShift();
    result.fold(
      (f) => emit(AttendanceFailure(f)),
      (shift) => emit(AttendanceLoaded(shift: shift)),
    );
  }

  Future<void> _onCheckIn(CheckInRequested event, Emitter<AttendanceState> emit) async {
    final current = state is AttendanceLoaded ? state as AttendanceLoaded : null;
    if (current?.shift == null) return;

    emit(const AttendanceLoading());
    final result = await checkIn(
      shiftId: current!.shift!.id,
      lat: event.lat,
      lng: event.lng,
      deviceId: event.deviceId,
    );
    result.fold(
      (f) => emit(AttendanceFailure(f)),
      (record) {
        // Emit the terminal state first (drives the one-time success snackbar), then fall
        // back to AttendanceLoaded with the new open record so the screen can show a working
        // Check Out button instead of getting stuck on a spinner.
        emit(AttendanceCheckedIn(record));
        emit(AttendanceLoaded(
          shift: current.shift,
          openRecord: record,
          currentLat: current.currentLat,
          currentLng: current.currentLng,
          distanceToShift: current.distanceToShift,
        ));
      },
    );
  }

  Future<void> _onCheckOut(CheckOutRequested event, Emitter<AttendanceState> emit) async {
    final current = state is AttendanceLoaded ? state as AttendanceLoaded : null;

    emit(const AttendanceLoading());
    final result = await checkOut(lat: current?.currentLat, lng: current?.currentLng);
    result.fold(
      (f) => emit(AttendanceFailure(f)),
      (_) {
        emit(const AttendanceCheckedOut());
        emit(AttendanceLoaded(
          shift: current?.shift,
          openRecord: null,
          currentLat: current?.currentLat,
          currentLng: current?.currentLng,
          distanceToShift: current?.distanceToShift,
        ));
      },
    );
  }

  void _onLocationRefreshed(LocationRefreshed event, Emitter<AttendanceState> emit) {
    final current = state is AttendanceLoaded ? state as AttendanceLoaded : null;
    if (current == null) return;

    double? distance;
    if (current.shift != null) {
      distance = _haversine(
        event.lat, event.lng,
        current.shift!.latitude, current.shift!.longitude,
      );
    }

    emit(AttendanceLoaded(
      shift: current.shift,
      openRecord: current.openRecord,
      currentLat: event.lat,
      currentLng: event.lng,
      distanceToShift: distance,
    ));
  }

  double _haversine(double lat1, double lng1, double lat2, double lng2) {
    const r = 6371000.0;
    final dLat = _toRad(lat2 - lat1);
    final dLng = _toRad(lng2 - lng1);
    final a = sin(dLat / 2) * sin(dLat / 2) +
        cos(_toRad(lat1)) * cos(_toRad(lat2)) * sin(dLng / 2) * sin(dLng / 2);
    return r * 2 * atan2(sqrt(a), sqrt(1 - a));
  }

  double _toRad(double deg) => deg * pi / 180;
}
