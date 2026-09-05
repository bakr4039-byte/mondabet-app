import 'package:equatable/equatable.dart';

abstract class AttendanceEvent extends Equatable {
  const AttendanceEvent();
  @override
  List<Object?> get props => [];
}

class AttendanceStarted extends AttendanceEvent {
  const AttendanceStarted();
}

class CheckInRequested extends AttendanceEvent {
  final double lat;
  final double lng;
  final String deviceId;
  const CheckInRequested({required this.lat, required this.lng, required this.deviceId});
  @override
  List<Object?> get props => [lat, lng, deviceId];
}

class CheckOutRequested extends AttendanceEvent {
  const CheckOutRequested();
}

class LocationRefreshed extends AttendanceEvent {
  final double lat;
  final double lng;
  const LocationRefreshed({required this.lat, required this.lng});
  @override
  List<Object?> get props => [lat, lng];
}

/// Fired on app start, after every check-in/check-out, and automatically
/// whenever connectivity comes back - attempts to flush any actions queued
/// while offline.
class SyncRequested extends AttendanceEvent {
  const SyncRequested();
}
