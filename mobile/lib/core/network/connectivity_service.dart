import 'package:connectivity_plus/connectivity_plus.dart';

/// Thin wrapper around connectivity_plus so the rest of the app only ever
/// deals with "online or not" rather than the specific connectivity type
/// (wifi/mobile/ethernet/none/...).
class ConnectivityService {
  final Connectivity _connectivity;
  ConnectivityService([Connectivity? connectivity]) : _connectivity = connectivity ?? Connectivity();

  Future<bool> get isOnline async {
    final results = await _connectivity.checkConnectivity();
    return _hasConnection(results);
  }

  /// Emits true whenever the device transitions to having *some* connection
  /// (used by AttendanceBloc to trigger an automatic sync of queued actions).
  Stream<bool> get onConnectivityChanged =>
      _connectivity.onConnectivityChanged.map(_hasConnection);

  bool _hasConnection(List<ConnectivityResult> results) =>
      results.any((r) => r != ConnectivityResult.none);
}
