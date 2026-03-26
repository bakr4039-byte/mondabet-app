import 'dart:convert';
import 'dart:typed_data';

import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:local_auth/local_auth.dart';

import '../../domain/usecases/biometric_challenge_usecase.dart';
import '../../domain/usecases/biometric_verify_usecase.dart';
import '../../domain/usecases/login_usecase.dart';
import '../../domain/usecases/logout_usecase.dart';
import '../../domain/usecases/verify_mfa_usecase.dart';
import 'auth_event.dart';
import 'auth_state.dart';

const _deviceIdKey = 'device_id';

class AuthBloc extends Bloc<AuthEvent, AuthState> {
  final LoginUseCase loginUseCase;
  final VerifyMfaUseCase verifyMfaUseCase;
  final BiometricChallengeUseCase biometricChallengeUseCase;
  final BiometricVerifyUseCase biometricVerifyUseCase;
  final LogoutUseCase logoutUseCase;
  final FlutterSecureStorage storage;
  final LocalAuthentication _localAuth = LocalAuthentication();

  AuthBloc({
    required this.loginUseCase,
    required this.verifyMfaUseCase,
    required this.biometricChallengeUseCase,
    required this.biometricVerifyUseCase,
    required this.logoutUseCase,
    required this.storage,
  }) : super(const AuthInitial()) {
    on<AppStarted>(_onAppStarted);
    on<LoginRequested>(_onLoginRequested);
    on<MfaVerified>(_onMfaVerified);
    on<BiometricRequested>(_onBiometricRequested);
    on<LogoutRequested>(_onLogoutRequested);
  }

  Future<void> _onAppStarted(AppStarted event, Emitter<AuthState> emit) async {
    final token = await storage.read(key: 'access_token');
    if (token != null) {
      emit(const AuthAuthenticated());
    } else {
      emit(const AuthUnauthenticated());
    }
  }

  Future<void> _onLoginRequested(
    LoginRequested event,
    Emitter<AuthState> emit,
  ) async {
    emit(const AuthLoading());

    final result = await loginUseCase(
      identifier: event.identifier,
      password: event.password,
      tenantCode: event.tenantCode,
    );

    result.fold(
      (failure) => emit(AuthFailure(failure)),
      (loginResult) {
        if (loginResult.mfaRequired && loginResult.sessionToken != null) {
          emit(AuthMfaRequired(sessionToken: loginResult.sessionToken!));
        } else if (loginResult.tokens != null) {
          emit(const AuthAuthenticated());
        } else {
          emit(const AuthUnauthenticated());
        }
      },
    );
  }

  Future<void> _onMfaVerified(
    MfaVerified event,
    Emitter<AuthState> emit,
  ) async {
    emit(const AuthLoading());

    final result = await verifyMfaUseCase(
      sessionToken: event.sessionToken,
      otp: event.otp,
    );

    result.fold(
      (failure) => emit(AuthFailure(failure)),
      (_) => emit(const AuthAuthenticated()),
    );
  }

  Future<void> _onBiometricRequested(
    BiometricRequested event,
    Emitter<AuthState> emit,
  ) async {
    emit(const AuthLoading());

    // Step 1: verify with local biometrics
    final canCheck = await _localAuth.canCheckBiometrics;
    if (!canCheck) {
      emit(const AuthUnauthenticated());
      return;
    }

    final authenticated = await _localAuth.authenticate(
      localizedReason: 'Use biometric to sign in',
      options: const AuthenticationOptions(biometricOnly: true),
    );

    if (!authenticated) {
      emit(const AuthUnauthenticated());
      return;
    }

    // Step 2: get server challenge
    final challengeResult =
        await biometricChallengeUseCase(event.deviceId);

    await challengeResult.fold(
      (failure) async => emit(AuthFailure(failure)),
      (challenge) async {
        // Step 3: sign challenge with device private key (stub – in production
        // use platform channel to sign with key stored in Keystore/Secure Enclave)
        final signedChallenge = _stubSignChallenge(challenge);

        final verifyResult = await biometricVerifyUseCase(
          deviceId: event.deviceId,
          signedChallenge: signedChallenge,
        );

        verifyResult.fold(
          (failure) => emit(AuthFailure(failure)),
          (_) => emit(const AuthAuthenticated()),
        );
      },
    );
  }

  Future<void> _onLogoutRequested(
    LogoutRequested event,
    Emitter<AuthState> emit,
  ) async {
    await logoutUseCase();
    emit(const AuthUnauthenticated());
  }

  /// Stub: in production, sign via platform channel using Keystore/Secure Enclave.
  String _stubSignChallenge(String challenge) =>
      base64Encode(Uint8List.fromList(utf8.encode('signed:$challenge')));
}
