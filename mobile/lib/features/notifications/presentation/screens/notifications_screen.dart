import 'package:easy_localization/easy_localization.dart';
import 'package:flutter/material.dart';

class NotificationsScreen extends StatelessWidget {
  const NotificationsScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text('notifications.title'.tr())),
      // Notification list is populated from FCM inbox – stub until backend inbox API is available
      body: Center(child: Text('notifications.empty'.tr())),
    );
  }
}
