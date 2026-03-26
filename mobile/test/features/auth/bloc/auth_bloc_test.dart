import 'package:bloc_test/bloc_test.dart';
import 'package:dartz/dartz.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mondabet/core/error/failures.dart';
import 'package:mondabet/features/auth/domain/entities/auth_tokens.dart';
import 'package:mondabet/features/auth/domain/usecases/biometric_challenge_usecase.dart';
import 'package:mondabet/features/auth/domain/usecases/biometric_verify_usecase.dart';
import 'package:mondabet/features/auth/domain/usecases/login_usecase.dart';
import 'package:mondabet/features/auth/domain/usecases/logout_usecase.dart';
import 'package:mondabet/features/auth/domain/usecases/verify_mfa_usecase.dart';
import 'package:mondabet/features/auth/presentation/bloc/auth_bloc.dart';
import 'package:mondabet/features/auth/presentation/bloc/auth_event.dart';
import 'package:mondabet/features/auth/presentation/bloc/auth_state.dart';

class MockLoginUseCase extends Mock implements LoginUseCase {}
class MockVerifyMfaUseCase extends Mock implements VerifyMfaUseCase {}
class MockBiometricChallengeUseCase extends Mock implements BiometricChallengeUseCase {}
class MockBiometricVerifyUseCase extends Mock implements BiometricVerifyUseCase {}
class MockLogoutUseCase extends Mock implements LogoutUseCase {}
class MockFlutterSecureStorage extends Mock implements FlutterSecureStorage {}

void main() {
  late MockLoginUseCase loginUseCase;
  late MockVerifyMfaUseCase verifyMfaUseCase;
  late MockBiometricChallengeUseCase biometricChallengeUseCase;
  late MockBiometricVerifyUseCase biometricVerifyUseCase;
  late MockLogoutUseCase logoutUseCase;
  late MockFlutterSecureStorage storage;

  setUp(() {
    loginUseCase = MockLoginUseCase();
    verifyMfaUseCase = MockVerifyMfaUseCase();
    biometricChallengeUseCase = MockBiometricChallengeUseCase();
    biometricVerifyUseCase = MockBiometricVerifyUseCase();
    logoutUseCase = MockLogoutUseCase();
    storage = MockFlutterSecureStorage();
  });

  AuthBloc buildBloc() => AuthBloc(
        loginUseCase: loginUseCase,
        verifyMfaUseCase: verifyMfaUseCase,
        biometricChallengeUseCase: biometricChallengeUseCase,
        biometricVerifyUseCase: biometricVerifyUseCase,
        logoutUseCase: logoutUseCase,
        storage: storage,
      );

  group('AppStarted', () {
    blocTest<AuthBloc, AuthState>(
      'emits AuthAuthenticated when token exists',
      build: buildBloc,
      setUp: () {
        when(() => storage.read(key: 'access_token'))
            .thenAnswer((_) async => 'some-token');
      },
      act: (bloc) => bloc.add(const AppStarted()),
      expect: () => [const AuthAuthenticated()],
    );

    blocTest<AuthBloc, AuthState>(
      'emits AuthUnauthenticated when no token',
      build: buildBloc,
      setUp: () {
        when(() => storage.read(key: 'access_token'))
            .thenAnswer((_) async => null);
      },
      act: (bloc) => bloc.add(const AppStarted()),
      expect: () => [const AuthUnauthenticated()],
    );
  });

  group('LoginRequested', () {
    blocTest<AuthBloc, AuthState>(
      'emits AuthMfaRequired when mfaRequired=true',
      build: buildBloc,
      setUp: () {
        when(() => loginUseCase(
              identifier: 'user@test.com',
              password: 'pass',
              tenantCode: 'tenant1',
            )).thenAnswer((_) async => const Right(
              LoginResult(mfaRequired: true, sessionToken: 'session-abc'),
            ));
      },
      act: (bloc) => bloc.add(const LoginRequested(
        identifier: 'user@test.com',
        password: 'pass',
        tenantCode: 'tenant1',
      )),
      expect: () => [
        const AuthLoading(),
        const AuthMfaRequired(sessionToken: 'session-abc'),
      ],
    );

    blocTest<AuthBloc, AuthState>(
      'emits AuthFailure on invalid credentials',
      build: buildBloc,
      setUp: () {
        when(() => loginUseCase(
              identifier: any(named: 'identifier'),
              password: any(named: 'password'),
              tenantCode: any(named: 'tenantCode'),
            )).thenAnswer(
          (_) async => const Left(UnauthorizedFailure('Invalid credentials.')),
        );
      },
      act: (bloc) => bloc.add(const LoginRequested(
        identifier: 'bad@test.com',
        password: 'wrong',
        tenantCode: 'tenant1',
      )),
      expect: () => [
        const AuthLoading(),
        const AuthFailure(UnauthorizedFailure('Invalid credentials.')),
      ],
    );
  });

  group('MfaVerified', () {
    const tokens = AuthTokens(
      accessToken: 'access-token',
      refreshToken: 'refresh-token',
    );

    blocTest<AuthBloc, AuthState>(
      'emits AuthAuthenticated on valid OTP',
      build: buildBloc,
      setUp: () {
        when(() => verifyMfaUseCase(
              sessionToken: 'session-abc',
              otp: '123456',
            )).thenAnswer((_) async => const Right(tokens));
      },
      act: (bloc) => bloc.add(
        const MfaVerified(sessionToken: 'session-abc', otp: '123456'),
      ),
      expect: () => [const AuthLoading(), const AuthAuthenticated()],
    );

    blocTest<AuthBloc, AuthState>(
      'emits AuthFailure on wrong OTP',
      build: buildBloc,
      setUp: () {
        when(() => verifyMfaUseCase(
              sessionToken: any(named: 'sessionToken'),
              otp: any(named: 'otp'),
            )).thenAnswer(
          (_) async =>
              const Left(UnauthorizedFailure('Invalid or expired OTP.')),
        );
      },
      act: (bloc) => bloc.add(
        const MfaVerified(sessionToken: 'session-abc', otp: '000000'),
      ),
      expect: () => [
        const AuthLoading(),
        const AuthFailure(UnauthorizedFailure('Invalid or expired OTP.')),
      ],
    );
  });

  group('LogoutRequested', () {
    blocTest<AuthBloc, AuthState>(
      'emits AuthUnauthenticated after logout',
      build: buildBloc,
      setUp: () {
        when(() => logoutUseCase()).thenAnswer((_) async => const Right(unit));
      },
      act: (bloc) => bloc.add(const LogoutRequested()),
      expect: () => [const AuthUnauthenticated()],
    );
  });
}
