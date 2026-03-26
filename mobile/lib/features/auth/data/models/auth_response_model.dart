import '../../domain/entities/auth_tokens.dart';

class LoginResponseModel {
  final String? accessToken;
  final String? refreshToken;
  final bool mfaRequired;
  final String? sessionToken;

  const LoginResponseModel({
    this.accessToken,
    this.refreshToken,
    required this.mfaRequired,
    this.sessionToken,
  });

  factory LoginResponseModel.fromJson(Map<String, dynamic> json) =>
      LoginResponseModel(
        accessToken: json['accessToken'] as String?,
        refreshToken: json['refreshToken'] as String?,
        mfaRequired: json['mfaRequired'] as bool? ?? false,
        sessionToken: json['sessionToken'] as String?,
      );

  LoginResult toDomain() => LoginResult(
        mfaRequired: mfaRequired,
        sessionToken: sessionToken,
        tokens: accessToken != null && refreshToken != null
            ? AuthTokens(
                accessToken: accessToken!,
                refreshToken: refreshToken!,
              )
            : null,
      );
}

class AuthTokensModel {
  final String accessToken;
  final String refreshToken;

  const AuthTokensModel({
    required this.accessToken,
    required this.refreshToken,
  });

  factory AuthTokensModel.fromJson(Map<String, dynamic> json) =>
      AuthTokensModel(
        accessToken: json['accessToken'] as String,
        refreshToken: json['refreshToken'] as String,
      );

  AuthTokens toDomain() =>
      AuthTokens(accessToken: accessToken, refreshToken: refreshToken);
}
