import 'package:dartz/dartz.dart';

import '../../../../core/error/failures.dart';
import '../repositories/auth_repository.dart';

class BiometricChallengeUseCase {
  final AuthRepository _repository;
  const BiometricChallengeUseCase(this._repository);

  Future<Either<Failure, String>> call(String deviceId) =>
      _repository.getBiometricChallenge(deviceId);
}
