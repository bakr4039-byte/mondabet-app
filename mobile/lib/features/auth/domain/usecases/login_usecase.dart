import 'package:dartz/dartz.dart';

import '../../../../core/error/failures.dart';
import '../entities/auth_tokens.dart';
import '../repositories/auth_repository.dart';

class LoginUseCase {
  final AuthRepository _repository;
  const LoginUseCase(this._repository);

  Future<Either<Failure, LoginResult>> call({
    required String identifier,
    required String password,
    required String tenantCode,
  }) =>
      _repository.login(
        identifier: identifier,
        password: password,
        tenantCode: tenantCode,
      );
}
