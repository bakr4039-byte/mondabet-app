import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

import '../../../../core/error/failures.dart';
import '../../domain/entities/auth_tokens.dart';
import '../../domain/repositories/auth_repository.dart';
import '../datasources/auth_remote_datasource.dart';

const _accessTokenKey = 'access_token';
const _refreshTokenKey = 'refresh_token';

class AuthRepositoryImpl implements AuthRepository {
  final AuthRemoteDataSource remoteDataSource;
  final FlutterSecureStorage storage;

  const AuthRepositoryImpl({
    required this.remoteDataSource,
    required this.storage,
  });

  @override
  Future<Either<Failure, LoginResult>> login({
    required String identifier,
    required String password,
    required String tenantCode,
  }) async {
    try {
      final model = await remoteDataSource.login(identifier, password, tenantCode);
      return Right(model.toDomain());
    } on DioException catch (e) {
      return Left(_mapDioError(e));
    } catch (_) {
      return const Left(NetworkFailure());
    }
  }

  @override
  Future<Either<Failure, AuthTokens>> verifyMfa({
    required String sessionToken,
    required String otp,
  }) async {
    try {
      final model = await remoteDataSource.verifyMfa(sessionToken, otp);
      final tokens = model.toDomain();
      await _storeTokens(tokens);
      return Right(tokens);
    } on DioException catch (e) {
      return Left(_mapDioError(e));
    } catch (_) {
      return const Left(NetworkFailure());
    }
  }

  @override
  Future<Either<Failure, String>> getBiometricChallenge(String deviceId) async {
    try {
      final challenge = await remoteDataSource.getBiometricChallenge(deviceId);
      return Right(challenge);
    } on DioException catch (e) {
      return Left(_mapDioError(e));
    } catch (_) {
      return const Left(NetworkFailure());
    }
  }

  @override
  Future<Either<Failure, AuthTokens>> verifyBiometric({
    required String deviceId,
    required String signedChallenge,
  }) async {
    try {
      final model = await remoteDataSource.verifyBiometric(deviceId, signedChallenge);
      final tokens = model.toDomain();
      await _storeTokens(tokens);
      return Right(tokens);
    } on DioException catch (e) {
      return Left(_mapDioError(e));
    } catch (_) {
      return const Left(NetworkFailure());
    }
  }

  @override
  Future<Either<Failure, Unit>> logout() async {
    try {
      await remoteDataSource.logout();
      await storage.deleteAll();
      return const Right(unit);
    } catch (_) {
      await storage.deleteAll();
      return const Right(unit);
    }
  }

  @override
  Future<bool> isAuthenticated() async {
    final token = await storage.read(key: _accessTokenKey);
    return token != null;
  }

  Future<void> _storeTokens(AuthTokens tokens) async {
    await storage.write(key: _accessTokenKey, value: tokens.accessToken);
    await storage.write(key: _refreshTokenKey, value: tokens.refreshToken);
  }

  Failure _mapDioError(DioException e) {
    if (e.type == DioExceptionType.connectionError ||
        e.type == DioExceptionType.connectionTimeout) {
      return const NetworkFailure();
    }
    final statusCode = e.response?.statusCode;
    final errorData = e.response?.data;
    final code = errorData?['error']?['code'] as String? ?? 'Unknown';
    final message = errorData?['error']?['message'] as String? ?? e.message ?? 'Unknown error';

    return switch (statusCode) {
      401 => UnauthorizedFailure(message),
      400 => ValidationFailure(message),
      _ => ServerFailure(code, message),
    };
  }
}
