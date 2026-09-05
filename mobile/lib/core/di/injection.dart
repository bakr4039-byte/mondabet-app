import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:get_it/get_it.dart';

import '../../features/attendance/data/datasources/attendance_local_datasource.dart';
import '../../features/attendance/data/datasources/attendance_remote_datasource.dart';
import '../../features/attendance/data/repositories/attendance_repository_impl.dart';
import '../../features/attendance/domain/repositories/attendance_repository.dart';
import '../../features/attendance/domain/usecases/check_in_usecase.dart';
import '../../features/attendance/domain/usecases/check_out_usecase.dart';
import '../../features/attendance/domain/usecases/get_current_shift_usecase.dart';
import '../../features/attendance/domain/usecases/get_pending_attendance_count_usecase.dart';
import '../../features/attendance/domain/usecases/sync_pending_attendance_usecase.dart';
import '../../features/attendance/presentation/bloc/attendance_bloc.dart';
import '../../features/auth/data/datasources/auth_remote_datasource.dart';
import '../../features/auth/data/repositories/auth_repository_impl.dart';
import '../../features/auth/domain/repositories/auth_repository.dart';
import '../../features/auth/domain/usecases/biometric_challenge_usecase.dart';
import '../../features/auth/domain/usecases/biometric_verify_usecase.dart';
import '../../features/auth/domain/usecases/login_usecase.dart';
import '../../features/auth/domain/usecases/logout_usecase.dart';
import '../../features/auth/domain/usecases/verify_mfa_usecase.dart';
import '../../features/auth/presentation/bloc/auth_bloc.dart';
import '../../features/leave/data/datasources/leave_remote_datasource.dart';
import '../../features/leave/data/repositories/leave_repository_impl.dart';
import '../../features/leave/domain/repositories/leave_repository.dart';
import '../../features/leave/domain/usecases/get_my_leaves_usecase.dart';
import '../../features/leave/domain/usecases/submit_leave_usecase.dart';
import '../../features/leave/presentation/bloc/leave_bloc.dart';
import '../../features/reports/data/datasources/report_remote_datasource.dart';
import '../../features/reports/data/repositories/report_repository_impl.dart';
import '../../features/reports/domain/repositories/report_repository.dart';
import '../../features/reports/presentation/bloc/reports_bloc.dart';
import '../network/api_client.dart';
import '../network/connectivity_service.dart';
import '../services/fcm_service.dart';

final getIt = GetIt.instance;

void configureDependencies() {
  // Core
  getIt.registerSingleton<FlutterSecureStorage>(
    const FlutterSecureStorage(
      aOptions: AndroidOptions(encryptedSharedPreferences: true),
    ),
  );
  getIt.registerSingleton<ApiClient>(ApiClient(getIt()));
  getIt.registerSingleton<FcmService>(FcmService());
  getIt.registerSingleton<ConnectivityService>(ConnectivityService());

  // Auth
  getIt.registerLazySingleton<AuthRemoteDataSource>(
    () => AuthRemoteDataSourceImpl(getIt<ApiClient>().dio),
  );
  getIt.registerLazySingleton<AuthRepository>(
    () => AuthRepositoryImpl(remoteDataSource: getIt(), storage: getIt()),
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

  // Attendance
  getIt.registerLazySingleton<AttendanceRemoteDataSource>(
    () => AttendanceRemoteDataSourceImpl(getIt<ApiClient>().dio),
  );
  getIt.registerLazySingleton<AttendanceLocalDataSource>(
    () => AttendanceLocalDataSourceImpl(),
  );
  getIt.registerLazySingleton<AttendanceRepository>(
    () => AttendanceRepositoryImpl(getIt(), getIt(), getIt()),
  );
  getIt.registerLazySingleton(() => GetCurrentShiftUseCase(getIt()));
  getIt.registerLazySingleton(() => CheckInUseCase(getIt()));
  getIt.registerLazySingleton(() => CheckOutUseCase(getIt()));
  getIt.registerLazySingleton(() => SyncPendingAttendanceUseCase(getIt()));
  getIt.registerLazySingleton(() => GetPendingAttendanceCountUseCase(getIt()));
  getIt.registerFactory<AttendanceBloc>(
    () => AttendanceBloc(
      getCurrentShift: getIt(),
      checkIn: getIt(),
      checkOut: getIt(),
      syncPending: getIt(),
      getPendingCount: getIt(),
      connectivityService: getIt(),
    ),
  );

  // Leave
  getIt.registerLazySingleton<LeaveRemoteDataSource>(
    () => LeaveRemoteDataSourceImpl(getIt<ApiClient>().dio),
  );
  getIt.registerLazySingleton<LeaveRepository>(
    () => LeaveRepositoryImpl(getIt()),
  );
  getIt.registerLazySingleton(() => SubmitLeaveUseCase(getIt()));
  getIt.registerLazySingleton(() => GetMyLeavesUseCase(getIt()));
  getIt.registerFactory<LeaveBloc>(
    () => LeaveBloc(submitLeave: getIt(), getMyLeaves: getIt()),
  );

  // Reports
  getIt.registerLazySingleton<ReportRemoteDataSource>(
    () => ReportRemoteDataSourceImpl(getIt<ApiClient>().dio),
  );
  getIt.registerLazySingleton<ReportRepository>(
    () => ReportRepositoryImpl(getIt()),
  );
  getIt.registerFactory<ReportsBloc>(() => ReportsBloc(getIt()));
}
