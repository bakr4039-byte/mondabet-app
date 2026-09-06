import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter_local_notifications/flutter_local_notifications.dart';

class FcmService {
  // Lazy on purpose: FirebaseMessaging.instance requires Firebase.initializeApp()
  // to have already run, but FcmService() itself is constructed eagerly by
  // configureDependencies() (registerSingleton) in main() - well before
  // Firebase.initializeApp() is called there. A field initializer here would
  // run at construction time regardless, crashing on startup on any build
  // where Firebase isn't configured yet, even though main() already wraps
  // Firebase setup in a try/catch specifically to make that case safe. Making
  // this lazy defers the actual FirebaseMessaging.instance call to first use
  // (inside initialize()/getToken(), which only ever run after Firebase.
  // initializeApp() has succeeded), so just constructing FcmService no longer
  // touches Firebase at all.
  FirebaseMessaging get _messaging => FirebaseMessaging.instance;
  final FlutterLocalNotificationsPlugin _localNotif =
      FlutterLocalNotificationsPlugin();

  Future<void> initialize() async {
    await _messaging.requestPermission(alert: true, badge: true, sound: true);

    const androidSettings = AndroidInitializationSettings('@mipmap/ic_launcher');
    const iosSettings = DarwinInitializationSettings();
    await _localNotif.initialize(
      const InitializationSettings(android: androidSettings, iOS: iosSettings),
    );

    // Foreground messages
    FirebaseMessaging.onMessage.listen(_showLocalNotification);
  }

  Future<String?> getToken() => _messaging.getToken();

  Future<void> _showLocalNotification(RemoteMessage message) async {
    const channel = AndroidNotificationChannel(
      'mondabet_channel',
      'Mondabet Notifications',
      importance: Importance.high,
    );

    await _localNotif.show(
      message.hashCode,
      message.notification?.title,
      message.notification?.body,
      NotificationDetails(
        android: AndroidNotificationDetails(channel.id, channel.name),
        iOS: const DarwinNotificationDetails(),
      ),
    );
  }
}
