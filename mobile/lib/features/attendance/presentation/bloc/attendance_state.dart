import 'package:equatable/equatable.dart';

import '../../../../core/error/failures.dart';
import '../../domain/entities/attendance_record.dart';
import '../../domain/entities/shift.dart';

abstract class AttendanceState extends Equatable {
  const AttendanceState();
  @override
  List<Object?> get props => [];
}

class AttendanceInitial extends AttendanceState {
  const AttendanceInitial();
}

class AttendanceLoading extends AttendanceState {
  const AttendanceLoading();
}

class AttendanceLoaded extends AttendanceState {
  final Shift? shift;
  final AttendanceRecord? openRecord;
  final double? currentLat;
  final double? currentLng;
  final double? distanceToShift;

  const AttendanceLoaded({
    this.shift,
    this.openRecord,
    this.currentLat,
    this.currentLng,
    this.distanceToShift,
  });

  bool get isWithinGeofence =>
      shift == null || distanceToShift == null
          ? false
          : distanceToShift! <= shift!.radiusMeters;

  @override
  List<Object?> get props => [shift, openRecord, currentLat, currentLng, distanceToShift];
}

class AttendanceCheckedIn extends AttendanceState {
  final AttendanceRecord record;
  const AttendanceCheckedIn(this.record);
  @override
  List<Object?> get props => [record];
}

class AttendanceCheckedOut extends AttendanceState {
  const AttendanceCheckedOut();
}

class AttendanceFailure extends AttendanceState {
  final Failure failure;
  const AttendanceFailure(this.failure);
  @override
  List<Object?> get props => [failure];
}
