import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:get_it/get_it.dart';
import 'package:injectable/injectable.dart';

import '../../features/auth/data/datasources/auth_remote_datasource.dart';
import '../../features/auth/data/repositories/auth_repository_impl.dart';
import '../../features/auth/domain/repositories/auth_repository.dart';
import '../../features/auth/domain/usecases/biometric_challenge_usecase.dart';
import '../../features/auth/domain/usecases/biometric_verify_usecase.dart';
import '../../features/auth/domain/usecases/login_usecase.dart';
import '../../features/auth/domain/usecases/logout_usecase.dart';
import '../../features/auth/domain/usecases/verify_mfa_usecase.dart';
import '../../features/auth/presentation/bloc/auth_bloc.dart';
import '../network/api_client.dart';

final getIt = GetIt.instance;

void configureDependencies() {
  // Core
  getIt.registerSingleton<FlutterSecureStorage>(
    const FlutterSecureStorage(
      aOptions: AndroidOptions(encryptedSharedPreferences: true),
    ),
  );

  getIt.registerSingleton<ApiClient>(ApiClient(getIt()));

  // Auth
  getIt.registerLazySingleton<AuthRemoteDataSource>(
    () => AuthRemoteDataSourceImpl(getIt<ApiClient>().dio),
  );

  getIt.registerLazySingleton<AuthRepository>(
    () => AuthRepositoryImpl(
      remoteDataSource: getIt(),
      storage: getIt(),
    ),
  );

  getIt.registerLazySingleton(() => LoginUseCase(getIt()));
  getIt.registerLazySingleton(() => VerifyMfaUseCase(getIt()));
  getIt.registerLazySingleton(() => BiometricChallengeUseCase(getIt()));
  getIt.registerLazySingleton(() => BiometricVerifyUseCase(getIt()));
  getIt.registerLazySingleton(() => LogoutUseCase(getIt()));

  getIt.registerFactory<AuthBloc>(
    () => AuthBloc(
      loginUseCase: getIt(),
      verifyMfaUseCase: getIt(),
      biometricChallengeUseCase: getIt(),
      biometricVerifyUseCase: getIt(),
      logoutUseCase: getIt(),
      storage: getIt(),
    ),
  );
}
