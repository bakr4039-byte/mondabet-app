import 'package:equatable/equatable.dart';

class AuthTokens extends Equatable {
  final String accessToken;
  final String refreshToken;

  const AuthTokens({required this.accessToken, required this.refreshToken});

  @override
  List<Object> get props => [accessToken, refreshToken];
}

class LoginResult extends Equatable {
  final AuthTokens? tokens;
  final bool mfaRequired;
  final String? sessionToken;

  const LoginResult({
    this.tokens,
    required this.mfaRequired,
    this.sessionToken,
  });

  @override
  List<Object?> get props => [tokens, mfaRequired, sessionToken];
}
