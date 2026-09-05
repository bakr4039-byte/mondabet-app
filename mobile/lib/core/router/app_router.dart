import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../features/auth/presentation/screens/login_screen.dart';
import '../../features/auth/presentation/screens/mfa_screen.dart';
import '../../features/auth/presentation/screens/splash_screen.dart';
import '../../features/clarifications/domain/entities/clarification.dart';
import '../../features/clarifications/presentation/screens/clarification_response_screen.dart';
import '../../features/home/presentation/screens/home_screen.dart';
import '../../features/leave/presentation/screens/excuse_form_screen.dart';
import '../../features/leave/presentation/screens/permission_form_screen.dart';
import '../../features/leave/presentation/screens/vacation_form_screen.dart';
import 'app_routes.dart';

export 'app_routes.dart';

class AppRouter {
  static final router = GoRouter(
    initialLocation: AppRoutes.splash,
    routes: [
      GoRoute(
        path: AppRoutes.splash,
        builder: (_, __) => const SplashScreen(),
      ),
      GoRoute(
        path: AppRoutes.login,
        builder: (_, __) => const LoginScreen(),
      ),
      GoRoute(
        path: AppRoutes.mfa,
        builder: (context, state) {
          final sessionToken = state.extra as String;
          return MfaScreen(sessionToken: sessionToken);
        },
      ),
      GoRoute(
        path: AppRoutes.home,
        builder: (_, __) => const HomeScreen(),
      ),
      GoRoute(
        path: AppRoutes.leaveVacation,
        builder: (_, __) => const VacationFormScreen(),
      ),
      GoRoute(
        path: AppRoutes.leavePermission,
        builder: (_, __) => const PermissionFormScreen(),
      ),
      GoRoute(
        path: AppRoutes.leaveExcuse,
        builder: (_, __) => const ExcuseFormScreen(),
      ),
      GoRoute(
        path: AppRoutes.clarificationRespond,
        builder: (_, state) {
          final c = state.extra as ClarificationItem;
          return ClarificationResponseScreen(clarification: c);
        },
      ),
    ],
    errorBuilder: (context, state) => Scaffold(
      body: Center(child: Text('Route not found: ${state.uri}')),
    ),
  );
}
