import 'package:bloc_test/bloc_test.dart';
import 'package:dartz/dartz.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';

import 'package:mondabet/core/error/failures.dart';
import 'package:mondabet/core/network/connectivity_service.dart';
import 'package:mondabet/features/attendance/domain/entities/attendance_record.dart';
import 'package:mondabet/features/attendance/domain/entities/shift.dart';
import 'package:mondabet/features/attendance/domain/usecases/check_in_usecase.dart';
import 'package:mondabet/features/attendance/domain/usecases/check_out_usecase.dart';
import 'package:mondabet/features/attendance/domain/usecases/get_current_shift_usecase.dart';
import 'package:mondabet/features/attendance/domain/usecases/get_pending_attendance_count_usecase.dart';
import 'package:mondabet/features/attendance/domain/usecases/sync_pending_attendance_usecase.dart';
import 'package:mondabet/features/attendance/presentation/bloc/attendance_bloc.dart';
import 'package:mondabet/features/attendance/presentation/bloc/attendance_event.dart';
import 'package:mondabet/features/attendance/presentation/bloc/attendance_state.dart';

class MockGetCurrentShift extends Mock implements GetCurrentShiftUseCase {}
class MockCheckIn extends Mock implements CheckInUseCase {}
class MockCheckOut extends Mock implements CheckOutUseCase {}
class MockSyncPending extends Mock implements SyncPendingAttendanceUseCase {}
class MockGetPendingCount extends Mock implements GetPendingAttendanceCountUseCase {}
class MockConnectivityService extends Mock implements ConnectivityService {}

const _shift = Shift(
  id: 'shift-1',
  name: 'Morning Shift',
  startTime: '08:00',
  endTime: '17:00',
  latitude: 24.7136,
  longitude: 46.6753,
  radiusMeters: 100,
  daysOfWeek: [1, 2, 3, 4, 5],
);

final _record = AttendanceRecord(
  id: 'rec-1',
  employeeId: 'emp-1',
  shiftId: 'shift-1',
  checkInTime: DateTime(2025, 1, 1, 8, 0),
  checkInLat: 24.714,
  checkInLng: 46.675,
  isWithinGeofence: true,
);

void main() {
  late MockGetCurrentShift mockGetShift;
  late MockCheckIn mockCheckIn;
  late MockCheckOut mockCheckOut;
  late MockSyncPending mockSyncPending;
  late MockGetPendingCount mockGetPendingCount;
  late MockConnectivityService mockConnectivity;

  setUp(() {
    mockGetShift = MockGetCurrentShift();
    mockCheckIn = MockCheckIn();
    mockCheckOut = MockCheckOut();
    mockSyncPending = MockSyncPending();
    mockGetPendingCount = MockGetPendingCount();
    mockConnectivity = MockConnectivityService();

    registerFallbackValue(const CheckInRequested(lat: 0, lng: 0, deviceId: ''));

    // Defaults so tests that don't care about offline sync aren't affected by it.
    when(() => mockConnectivity.onConnectivityChanged).thenAnswer((_) => const Stream.empty());
    when(() => mockGetPendingCount()).thenAnswer((_) async => 0);
    when(() => mockSyncPending()).thenAnswer((_) async => const Right(0));
  });

  AttendanceBloc build() => AttendanceBloc(
        getCurrentShift: mockGetShift,
        checkIn: mockCheckIn,
        checkOut: mockCheckOut,
        syncPending: mockSyncPending,
        getPendingCount: mockGetPendingCount,
        connectivityService: mockConnectivity,
      );

  group('AttendanceStarted', () {
    blocTest<AttendanceBloc, AttendanceState>(
      'emits [Loading, Loaded] when shift loaded',
      build: build,
      setUp: () => when(() => mockGetShift()).thenAnswer((_) async => const Right(_shift)),
      act: (b) => b.add(const AttendanceStarted()),
      expect: () => [
        const AttendanceLoading(),
        isA<AttendanceLoaded>(),
      ],
    );

    blocTest<AttendanceBloc, AttendanceState>(
      'emits [Loading, Failure] on error',
      build: build,
      setUp: () => when(() => mockGetShift())
          .thenAnswer((_) async => Left(ServerFailure('error'))),
      act: (b) => b.add(const AttendanceStarted()),
      expect: () => [
        const AttendanceLoading(),
        isA<AttendanceFailure>(),
      ],
    );

    blocTest<AttendanceBloc, AttendanceState>(
      'triggers an automatic sync when actions are queued from a previous offline session',
      build: build,
      setUp: () {
        when(() => mockGetShift()).thenAnswer((_) async => const Right(_shift));
        when(() => mockGetPendingCount()).thenAnswer((_) async => 2);
      },
      act: (b) => b.add(const AttendanceStarted()),
      expect: () => [
        const AttendanceLoading(),
        isA<AttendanceLoaded>().having((s) => s.pendingCount, 'pendingCount', 2),
      ],
      verify: (_) => verify(() => mockSyncPending()).called(1),
    );
  });

  group('CheckInRequested', () {
    blocTest<AttendanceBloc, AttendanceState>(
      'emits [Loading, CheckedIn, Loaded] on success',
      build: build,
      seed: () => AttendanceLoaded(shift: _shift),
      setUp: () => when(() => mockCheckIn(
            shiftId: any(named: 'shiftId'),
            lat: any(named: 'lat'),
            lng: any(named: 'lng'),
            deviceId: any(named: 'deviceId'),
          )).thenAnswer((_) async => Right(_record)),
      act: (b) => b.add(const CheckInRequested(lat: 24.714, lng: 46.675, deviceId: 'd1')),
      expect: () => [
        const AttendanceLoading(),
        isA<AttendanceCheckedIn>(),
        isA<AttendanceLoaded>().having((s) => s.openRecord, 'openRecord', isNotNull),
      ],
    );

    blocTest<AttendanceBloc, AttendanceState>(
      'queues offline and still reaches Loaded with an open record marked pendingSync',
      build: build,
      seed: () => AttendanceLoaded(shift: _shift),
      setUp: () {
        when(() => mockCheckIn(
              shiftId: any(named: 'shiftId'),
              lat: any(named: 'lat'),
              lng: any(named: 'lng'),
              deviceId: any(named: 'deviceId'),
            )).thenAnswer((_) async => Right(AttendanceRecord(
              id: 'pending-1',
              employeeId: '',
              shiftId: 'shift-1',
              checkInTime: DateTime(2025, 1, 1, 8, 0),
              checkInLat: 24.714,
              checkInLng: 46.675,
              isWithinGeofence: true,
              pendingSync: true,
            )));
        when(() => mockGetPendingCount()).thenAnswer((_) async => 1);
      },
      act: (b) => b.add(const CheckInRequested(lat: 24.714, lng: 46.675, deviceId: 'd1')),
      expect: () => [
        const AttendanceLoading(),
        isA<AttendanceCheckedIn>().having((s) => s.record.pendingSync, 'pendingSync', true),
        isA<AttendanceLoaded>()
            .having((s) => s.pendingCount, 'pendingCount', 1)
            .having((s) => s.openRecord?.pendingSync, 'openRecord.pendingSync', true),
      ],
    );
  });

  group('LocationRefreshed', () {
    blocTest<AttendanceBloc, AttendanceState>(
      'updates distance and geofence status',
      build: build,
      seed: () => const AttendanceLoaded(shift: _shift),
      act: (b) => b.add(const LocationRefreshed(lat: 24.714, lng: 46.676)),
      expect: () => [
        isA<AttendanceLoaded>().having((s) => s.distanceToShift, 'distance', isNotNull),
      ],
    );
  });
}
