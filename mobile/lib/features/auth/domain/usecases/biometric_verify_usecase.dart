import 'package:dartz/dartz.dart';

import '../../../../core/error/failures.dart';
import '../entities/auth_tokens.dart';
import '../repositories/auth_repository.dart';

class BiometricVerifyUseCase {
  final AuthRepository _repository;
  const BiometricVerifyUseCase(this._repository);

  Future<Either<Failure, AuthTokens>> call({
    required String deviceId,
    required String signedChallenge,
  }) =>
      _repository.verifyBiometric(
        deviceId: deviceId,
        signedChallenge: signedChallenge,
      );
}
