import 'package:equatable/equatable.dart';

abstract class AuthEvent extends Equatable {
  const AuthEvent();

  @override
  List<Object?> get props => [];
}

class AppStarted extends AuthEvent {
  const AppStarted();
}

class LoginRequested extends AuthEvent {
  final String identifier;
  final String password;
  final String tenantCode;

  const LoginRequested({
    required this.identifier,
    required this.password,
    required this.tenantCode,
  });

  @override
  List<Object> get props => [identifier, tenantCode];
}

class MfaVerified extends AuthEvent {
  final String sessionToken;
  final String otp;

  const MfaVerified({required this.sessionToken, required this.otp});

  @override
  List<Object> get props => [sessionToken, otp];
}

class BiometricRequested extends AuthEvent {
  final String deviceId;

  const BiometricRequested({required this.deviceId});

  @override
  List<Object> get props => [deviceId];
}

class LogoutRequested extends AuthEvent {
  const LogoutRequested();
}
