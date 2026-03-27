import 'package:easy_localization/easy_localization.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:hive_flutter/hive_flutter.dart';

import 'core/di/injection.dart';
import 'core/router/app_router.dart';
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

  await getIt<FcmService>().initialize();

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
