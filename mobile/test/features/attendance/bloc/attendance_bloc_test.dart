import 'package:bloc_test/bloc_test.dart';
import 'package:dartz/dartz.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';

import 'package:mondabet/core/error/failures.dart';
import 'package:mondabet/features/attendance/domain/entities/attendance_record.dart';
import 'package:mondabet/features/attendance/domain/entities/shift.dart';
import 'package:mondabet/features/attendance/domain/usecases/check_in_usecase.dart';
import 'package:mondabet/features/attendance/domain/usecases/check_out_usecase.dart';
import 'package:mondabet/features/attendance/domain/usecases/get_current_shift_usecase.dart';
import 'package:mondabet/features/attendance/presentation/bloc/attendance_bloc.dart';
import 'package:mondabet/features/attendance/presentation/bloc/attendance_event.dart';
import 'package:mondabet/features/attendance/presentation/bloc/attendance_state.dart';

class MockGetCurrentShift extends Mock implements GetCurrentShiftUseCase {}
class MockCheckIn extends Mock implements CheckInUseCase {}
class MockCheckOut extends Mock implements CheckOutUseCase {}

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

  setUp(() {
    mockGetShift = MockGetCurrentShift();
    mockCheckIn = MockCheckIn();
    mockCheckOut = MockCheckOut();

    registerFallbackValue(const CheckInRequested(lat: 0, lng: 0, deviceId: ''));
  });

  AttendanceBloc build() => AttendanceBloc(
        getCurrentShift: mockGetShift,
        checkIn: mockCheckIn,
        checkOut: mockCheckOut,
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
  });

  group('CheckInRequested', () {
    blocTest<AttendanceBloc, AttendanceState>(
      'emits [Loading, CheckedIn] on success',
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
