import 'dart:async';
import 'dart:math';

import 'package:flutter_bloc/flutter_bloc.dart';

import '../../../../core/network/connectivity_service.dart';
import '../../domain/usecases/check_in_usecase.dart';
import '../../domain/usecases/check_out_usecase.dart';
import '../../domain/usecases/get_current_shift_usecase.dart';
import '../../domain/usecases/get_pending_attendance_count_usecase.dart';
import '../../domain/usecases/sync_pending_attendance_usecase.dart';
import 'attendance_event.dart';
import 'attendance_state.dart';

class AttendanceBloc extends Bloc<AttendanceEvent, AttendanceState> {
  final GetCurrentShiftUseCase getCurrentShift;
  final CheckInUseCase checkIn;
  final CheckOutUseCase checkOut;
  final SyncPendingAttendanceUseCase syncPending;
  final GetPendingAttendanceCountUseCase getPendingCount;
  final ConnectivityService connectivityService;

  StreamSubscription<bool>? _connectivitySub;

  AttendanceBloc({
    required this.getCurrentShift,
    required this.checkIn,
    required this.checkOut,
    required this.syncPending,
    required this.getPendingCount,
    required this.connectivityService,
  }) : super(const AttendanceInitial()) {
    on<AttendanceStarted>(_onStarted);
    on<CheckInRequested>(_onCheckIn);
    on<CheckOutRequested>(_onCheckOut);
    on<LocationRefreshed>(_onLocationRefreshed);
    on<SyncRequested>(_onSync);

    // Whenever the device regains connectivity, automatically try to flush
    // anything queued while offline - no manual "sync now" tap required.
    _connectivitySub = connectivityService.onConnectivityChanged
        .where((online) => online)
        .listen((_) => add(const SyncRequested()));
  }

  Future<void> _onStarted(AttendanceStarted event, Emitter<AttendanceState> emit) async {
    emit(const AttendanceLoading());
    final result = await getCurrentShift();
    final pending = await getPendingCount();
    result.fold(
      (f) => emit(AttendanceFailure(f)),
      (shift) => emit(AttendanceLoaded(shift: shift, pendingCount: pending)),
    );
    if (pending > 0) add(const SyncRequested());
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
    final pending = await getPendingCount();
    result.fold(
      (f) => emit(AttendanceFailure(f)),
      (record) {
        // Emit the terminal state first (drives the one-time success snackbar), then fall
        // back to AttendanceLoaded with the new open record so the screen can show a working
        // Check Out button instead of getting stuck on a spinner. record.pendingSync tells
        // the UI whether this was a real server confirmation or an offline-queued action.
        emit(AttendanceCheckedIn(record));
        emit(AttendanceLoaded(
          shift: current.shift,
          openRecord: record,
          currentLat: current.currentLat,
          currentLng: current.currentLng,
          distanceToShift: current.distanceToShift,
          pendingCount: pending,
        ));
      },
    );
  }

  Future<void> _onCheckOut(CheckOutRequested event, Emitter<AttendanceState> emit) async {
    final current = state is AttendanceLoaded ? state as AttendanceLoaded : null;

    emit(const AttendanceLoading());
    final result = await checkOut(lat: current?.currentLat, lng: current?.currentLng);
    final pending = await getPendingCount();
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
          pendingCount: pending,
        ));
      },
    );
  }

  /// Flushes the offline queue. Fired on app start (if anything is queued),
  /// after every check-in/check-out, on the connectivity listener above, and
  /// from the manual "sync now" button on the check-in screen.
  Future<void> _onSync(SyncRequested event, Emitter<AttendanceState> emit) async {
    await syncPending();
    final pending = await getPendingCount();

    final current = state is AttendanceLoaded ? state as AttendanceLoaded : null;
    if (current == null || current.pendingCount == pending) return;

    emit(AttendanceLoaded(
      shift: current.shift,
      openRecord: current.openRecord,
      currentLat: current.currentLat,
      currentLng: current.currentLng,
      distanceToShift: current.distanceToShift,
      pendingCount: pending,
    ));
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
      pendingCount: current.pendingCount,
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

  @override
  Future<void> close() {
    _connectivitySub?.cancel();
    return super.close();
  }
}
