import 'package:dio/dio.dart';

import '../models/auth_response_model.dart';

abstract class AuthRemoteDataSource {
  Future<LoginResponseModel> login(String identifier, String password, String tenantCode);
  Future<AuthTokensModel> verifyMfa(String sessionToken, String otp);
  Future<String> getBiometricChallenge(String deviceId);
  Future<AuthTokensModel> verifyBiometric(String deviceId, String signedChallenge);
  Future<void> logout();
}

class AuthRemoteDataSourceImpl implements AuthRemoteDataSource {
  final Dio _dio;
  const AuthRemoteDataSourceImpl(this._dio);

  @override
  Future<LoginResponseModel> login(
    String identifier,
    String password,
    String tenantCode,
  ) async {
    final response = await _dio.post(
      '/api/v1/auth/login',
      data: {
        'identifier': identifier,
        'password': password,
        'tenantCode': tenantCode,
      },
    );
    return LoginResponseModel.fromJson(
      response.data['data'] as Map<String, dynamic>,
    );
  }

  @override
  Future<AuthTokensModel> verifyMfa(String sessionToken, String otp) async {
    final response = await _dio.post(
      '/api/v1/auth/mfa/verify',
      data: {'sessionToken': sessionToken, 'otp': otp},
    );
    return AuthTokensModel.fromJson(
      response.data['data'] as Map<String, dynamic>,
    );
  }

  @override
  Future<String> getBiometricChallenge(String deviceId) async {
    final response = await _dio.post(
      '/api/v1/auth/biometric/challenge',
      data: {'deviceId': deviceId},
    );
    return response.data['data']['challenge'] as String;
  }

  @override
  Future<AuthTokensModel> verifyBiometric(
    String deviceId,
    String signedChallenge,
  ) async {
    final response = await _dio.post(
      '/api/v1/auth/biometric/verify',
      data: {'deviceId': deviceId, 'signedChallenge': signedChallenge},
    );
    return AuthTokensModel.fromJson(
      response.data['data'] as Map<String, dynamic>,
    );
  }

  @override
  Future<void> logout() =>
      _dio.post('/api/v1/auth/logout');
}
