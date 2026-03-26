import 'package:equatable/equatable.dart';

import '../../../../core/error/failures.dart';

abstract class AuthState extends Equatable {
  const AuthState();

  @override
  List<Object?> get props => [];
}

class AuthInitial extends AuthState {
  const AuthInitial();
}

class AuthLoading extends AuthState {
  const AuthLoading();
}

class AuthUnauthenticated extends AuthState {
  const AuthUnauthenticated();
}

class AuthMfaRequired extends AuthState {
  final String sessionToken;

  const AuthMfaRequired({required this.sessionToken});

  @override
  List<Object> get props => [sessionToken];
}

class AuthAuthenticated extends AuthState {
  const AuthAuthenticated();
}

class AuthBiometricChallenge extends AuthState {
  final String deviceId;
  final String challenge;

  const AuthBiometricChallenge({
    required this.deviceId,
    required this.challenge,
  });

  @override
  List<Object> get props => [deviceId, challenge];
}

class AuthFailure extends AuthState {
  final Failure failure;

  const AuthFailure(this.failure);

  @override
  List<Object> get props => [failure];
}
