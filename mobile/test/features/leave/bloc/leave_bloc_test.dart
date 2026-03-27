import 'package:bloc_test/bloc_test.dart';
import 'package:dartz/dartz.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';

import 'package:mondabet/core/error/failures.dart';
import 'package:mondabet/features/leave/domain/entities/leave_request.dart';
import 'package:mondabet/features/leave/domain/usecases/get_my_leaves_usecase.dart';
import 'package:mondabet/features/leave/domain/usecases/submit_leave_usecase.dart';
import 'package:mondabet/features/leave/presentation/bloc/leave_bloc.dart';
import 'package:mondabet/features/leave/presentation/bloc/leave_event.dart';
import 'package:mondabet/features/leave/presentation/bloc/leave_state.dart';

class MockSubmitLeave extends Mock implements SubmitLeaveUseCase {}
class MockGetMyLeaves extends Mock implements GetMyLeavesUseCase {}

final _leave = LeaveRequest(
  id: 'leave-1',
  type: LeaveType.vacation,
  status: LeaveStatus.pending,
  fromDate: DateTime(2025, 3, 1),
  toDate: DateTime(2025, 3, 5),
  createdAt: DateTime(2025, 2, 28),
);

void main() {
  late MockSubmitLeave mockSubmit;
  late MockGetMyLeaves mockGetLeaves;

  setUp(() {
    mockSubmit = MockSubmitLeave();
    mockGetLeaves = MockGetMyLeaves();
  });

  LeaveBloc build() => LeaveBloc(submitLeave: mockSubmit, getMyLeaves: mockGetLeaves);

  group('LeavesLoaded', () {
    blocTest<LeaveBloc, LeaveState>(
      'emits [Loading, ListLoaded] on success',
      build: build,
      setUp: () => when(() => mockGetLeaves(type: any(named: 'type'), status: any(named: 'status')))
          .thenAnswer((_) async => Right([_leave])),
      act: (b) => b.add(const LeavesLoaded()),
      expect: () => [
        const LeaveLoading(),
        isA<LeaveListLoaded>().having((s) => s.leaves.length, 'count', 1),
      ],
    );

    blocTest<LeaveBloc, LeaveState>(
      'emits [Loading, Failure] on error',
      build: build,
      setUp: () => when(() => mockGetLeaves(type: any(named: 'type'), status: any(named: 'status')))
          .thenAnswer((_) async => Left(ServerFailure('error'))),
      act: (b) => b.add(const LeavesLoaded()),
      expect: () => [
        const LeaveLoading(),
        isA<LeaveFailure>(),
      ],
    );
  });

  group('LeaveSubmitted', () {
    blocTest<LeaveBloc, LeaveState>(
      'emits [Loading, SubmitSuccess] on vacation submit',
      build: build,
      setUp: () => when(() => mockSubmit(
            type: any(named: 'type'),
            fromDate: any(named: 'fromDate'),
            toDate: any(named: 'toDate'),
            reason: any(named: 'reason'),
            filePath: any(named: 'filePath'),
          )).thenAnswer((_) async => Right(_leave)),
      act: (b) => b.add(LeaveSubmitted(
        type: LeaveType.vacation,
        fromDate: DateTime(2025, 3, 1),
        toDate: DateTime(2025, 3, 5),
      )),
      expect: () => [
        const LeaveLoading(),
        isA<LeaveSubmitSuccess>(),
      ],
    );
  });
}
