import 'package:dartz/dartz.dart';

import '../../../../core/error/failures.dart';
import '../entities/auth_tokens.dart';

abstract class AuthRepository {
  Future<Either<Failure, LoginResult>> login({
    required String identifier,
    required String password,
    required String tenantCode,
  });

  Future<Either<Failure, AuthTokens>> verifyMfa({
    required String sessionToken,
    required String otp,
  });

  Future<Either<Failure, String>> getBiometricChallenge(String deviceId);

  Future<Either<Failure, AuthTokens>> verifyBiometric({
    required String deviceId,
    required String signedChallenge,
  });

  Future<Either<Failure, Unit>> logout();

  Future<bool> isAuthenticated();
}
