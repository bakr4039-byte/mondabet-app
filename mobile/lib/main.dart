import 'package:easy_localization/easy_localization.dart';
import 'package:firebase_core/firebase_core.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:hive_flutter/hive_flutter.dart';

import 'core/di/injection.dart';
import 'core/router/app_router.dart';
import 'core/services/fcm_service.dart';
import 'core/theme/tenant_config.dart';
import 'core/theme/theme_service.dart';
import 'features/attendance/presentation/bloc/attendance_bloc.dart';
import 'features/auth/presentation/bloc/auth_bloc.dart';
import 'features/leave/presentation/bloc/leave_bloc.dart';
import 'features/reports/presentation/bloc/reports_bloc.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();

  await Hive.initFlutter();
  await EasyLocalization.ensureInitialized();
  configureDependencies();

  // Firebase/FCM need a real Firebase project registered (google-services.json on
  // Android, GoogleService-Info.plist on iOS) before this succeeds - this was
  // previously never called at all, even though FcmService assumed it had been,
  // so push notifications silently could never have worked. Guarded so a build
  // without Firebase configured yet still launches normally, just without push
  // notifications, instead of crashing on startup.
  try {
    await Firebase.initializeApp();
    await getIt<FcmService>().initialize();
  } catch (e) {
    debugPrint('Firebase/FCM initialization skipped (not configured yet): $e');
  }

  runApp(
    EasyLocalization(
      supportedLocales: const [Locale('ar'), Locale('en'), Locale('ur')],
      path: 'assets/translations',
      fallbackLocale: const Locale('ar'),
      startLocale: const Locale('ar'),
      child: const MondabetApp(),
    ),
  );
}

class MondabetApp extends StatefulWidget {
  const MondabetApp({super.key});

  @override
  State<MondabetApp> createState() => _MondabetAppState();
}

class _MondabetAppState extends State<MondabetApp> {
  TenantConfig _config = TenantConfig.defaults();

  @override
  void initState() {
    super.initState();
    _loadTenantTheme();
  }

  Future<void> _loadTenantTheme() async {
    final saved = await TenantConfig.load();
    if (saved != null && mounted) {
      setState(() => _config = saved);
    }
  }

  @override
  Widget build(BuildContext context) {
    return MultiBlocProvider(
      providers: [
        BlocProvider(create: (_) => getIt<AuthBloc>()),
        BlocProvider(create: (_) => getIt<AttendanceBloc>()),
        BlocProvider(create: (_) => getIt<LeaveBloc>()),
        BlocProvider(create: (_) => getIt<ReportsBloc>()),
      ],
      child: MaterialApp.router(
        title: 'Mondabet',
        debugShowCheckedModeBanner: false,
        theme: ThemeService.buildFromConfig(_config),
        localizationsDelegates: context.localizationDelegates,
        supportedLocales: context.supportedLocales,
        locale: context.locale,
        routerConfig: AppRouter.router,
      ),
    );
  }
}
