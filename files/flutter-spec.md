# Flutter Mobile Spec – Mondabet Employee App

## Architecture
```
lib/
├── core/
│   ├── di/               # GetIt service locator
│   ├── network/          # Dio client, interceptors, token refresh
│   ├── error/            # Failure types, Result<T>
│   ├── theme/            # White-label ThemeData from tenant config
│   ├── l10n/             # ARB files (ar, en, ur)
│   └── router/           # go_router routes
├── features/
│   ├── auth/             # login, mfa, biometric, nafath
│   ├── attendance/       # check-in, GPS, shift info
│   ├── leave/            # vacation, permission, excuse
│   ├── reports/          # attendance history, download
│   ├── notifications/    # inbox, push
│   └── profile/          # my info, change password
└── main.dart
```

## Feature: Auth
Screens: SplashScreen → LoginScreen → MfaScreen → HomeScreen
BLoC: AuthBloc {events: LoginRequested, MfaVerified, BiometricRequested, NafathInitiated, LogoutRequested}
- Login fields: Iqama or username + password
- After success if mfaRequired → MfaScreen (6-digit OTP, resend 60s timer)
- Biometric: local_auth package → sign challenge from server
- Nafath: open Nafath deep-link or show QR; poll /auth/nafath/verify every 3s (max 60s)
- Store tokens: flutter_secure_storage (never SharedPreferences)

## Feature: Attendance (Check-In)
Screens: HomeScreen shows shift info + CheckInButton
BLoC: AttendanceBloc {events: CheckInRequested, CheckOutRequested, LocationRefreshed}
- geolocator package: request permission on first launch
- Validate geofence client-side first → show feedback → POST /checkins
- Show distance to office if outside geofence
- Real-time location indicator on map (google_maps_flutter)

## Feature: Leave
Screens: LeaveListScreen → LeaveDetailScreen, VacationFormScreen, PermissionFormScreen, ExcuseFormScreen
BLoC: LeaveBloc
- File upload: file_picker → multipart POST
- Workflow status shown as stepper widget
- Pull-to-refresh on leave list

## Feature: Reports
- Date range picker → GET /reports/attendance?format=pdf → open_file_plus to view
- Chart: fl_chart for monthly summary bars

## Feature: Notifications
- FCM integration (firebase_messaging)
- Local notification: flutter_local_notifications
- Deep-link to relevant screen on tap

## White-label Theming
```dart
// TenantTheme loaded from API on first launch, cached in Hive
ThemeData buildTenantTheme(TenantConfig cfg) => ThemeData(
  colorScheme: ColorScheme.fromSeed(seedColor: Color(int.parse(cfg.primaryColor.replaceAll('#','0xFF')))),
  ...
);
```
App logo and colors fetched from tenant-svc on splash, stored in Hive box.

## Key Packages
| Package | Purpose |
|---------|---------|
| flutter_bloc | State management |
| go_router | Navigation |
| dio | HTTP + interceptors |
| flutter_secure_storage | Token storage |
| local_auth | Biometric |
| geolocator | GPS |
| google_maps_flutter | Map display |
| file_picker | File upload |
| flutter_local_notifications | Push display |
| firebase_messaging | FCM |
| fl_chart | Charts |
| hive_flutter | Local cache |
| easy_localization | i18n (ARB) |
| mocktail | Testing mocks |
