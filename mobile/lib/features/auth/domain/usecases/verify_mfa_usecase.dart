import 'package:dartz/dartz.dart';

import '../../../../core/error/failures.dart';
import '../entities/auth_tokens.dart';
import '../repositories/auth_repository.dart';

class VerifyMfaUseCase {
  final AuthRepository _repository;
  const VerifyMfaUseCase(this._repository);

  Future<Either<Failure, AuthTokens>> call({
    required String sessionToken,
    required String otp,
  }) =>
      _repository.verifyMfa(sessionToken: sessionToken, otp: otp);
}
