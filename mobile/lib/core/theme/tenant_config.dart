import 'package:hive_flutter/hive_flutter.dart';

class TenantConfig {
  final String primaryColor;
  final String secondaryColor;
  final String logoUrl;
  final String tenantName;

  const TenantConfig({
    required this.primaryColor,
    required this.secondaryColor,
    required this.logoUrl,
    required this.tenantName,
  });

  factory TenantConfig.fromJson(Map<String, dynamic> json) => TenantConfig(
        primaryColor: json['primaryColor'] as String? ?? '#1976D2',
        secondaryColor: json['secondaryColor'] as String? ?? '#424242',
        logoUrl: json['logoUrl'] as String? ?? '',
        tenantName: json['name'] as String? ?? 'Mondabet',
      );

  factory TenantConfig.defaults() => const TenantConfig(
        primaryColor: '#1976D2',
        secondaryColor: '#424242',
        logoUrl: '',
        tenantName: 'Mondabet',
      );

  static const _boxName = 'tenant_config';
  static const _key = 'config';

  static Future<void> save(TenantConfig cfg) async {
    final box = await Hive.openBox<String>(_boxName);
    await box.put(_key, '${cfg.primaryColor}|${cfg.secondaryColor}|${cfg.logoUrl}|${cfg.tenantName}');
  }

  static Future<TenantConfig?> load() async {
    final box = await Hive.openBox<String>(_boxName);
    final raw = box.get(_key);
    if (raw == null) return null;
    final parts = raw.split('|');
    if (parts.length < 4) return null;
    return TenantConfig(
      primaryColor: parts[0],
      secondaryColor: parts[1],
      logoUrl: parts[2],
      tenantName: parts[3],
    );
  }
}
