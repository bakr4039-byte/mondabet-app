/// Route path constants only - no screen imports here on purpose, so any
/// screen can depend on this file (e.g. to push a route) without creating an
/// import cycle back through app_router.dart, which does import every screen.
abstract class AppRoutes {
  static const splash = '/';
  static const login = '/login';
  static const mfa = '/mfa';
  static const home = '/home';
  static const leaveVacation = '/leaves/vacation';
  static const leavePermission = '/leaves/permission';
  static const leaveExcuse = '/leaves/excuse';
  static const clarificationRespond = '/clarifications/respond';
}
