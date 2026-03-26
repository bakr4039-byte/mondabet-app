import 'package:equatable/equatable.dart';

abstract class Failure extends Equatable {
  final String message;
  const Failure(this.message);

  @override
  List<Object> get props => [message];
}

class NetworkFailure extends Failure {
  const NetworkFailure([super.message = 'Network error. Please check your connection.']);
}

class ServerFailure extends Failure {
  final String code;
  const ServerFailure(this.code, String message) : super(message);

  @override
  List<Object> get props => [code, message];
}

class UnauthorizedFailure extends Failure {
  const UnauthorizedFailure([super.message = 'Invalid credentials.']);
}

class ValidationFailure extends Failure {
  const ValidationFailure(super.message);
}

class BiometricFailure extends Failure {
  const BiometricFailure([super.message = 'Biometric authentication failed.']);
}

class StorageFailure extends Failure {
  const StorageFailure([super.message = 'Storage error.']);
}
