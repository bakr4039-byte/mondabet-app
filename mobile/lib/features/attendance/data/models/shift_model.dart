import 'dart:convert';

import '../../domain/entities/shift.dart';

class ShiftModel extends Shift {
  const ShiftModel({
    required super.id,
    required super.name,
    required super.startTime,
    required super.endTime,
    required super.latitude,
    required super.longitude,
    required super.radiusMeters,
    required super.daysOfWeek,
  });

  factory ShiftModel.fromJson(Map<String, dynamic> json) => ShiftModel(
        id: json['id'] as String,
        name: json['name'] as String,
        startTime: json['startTime'] as String,
        endTime: json['endTime'] as String,
        latitude: (json['latitude'] as num).toDouble(),
        longitude: (json['longitude'] as num).toDouble(),
        radiusMeters: json['radiusMeters'] as int,
        daysOfWeek: List<int>.from(
          jsonDecode(json['daysOfWeekJson'] as String? ?? '[]') as List,
        ),
      );

  /// Round-trips through the same shape fromJson expects - used only to cache
  /// the shift locally (Hive) for offline use, never sent back to the server.
  Map<String, dynamic> toJson() => {
        'id': id,
        'name': name,
        'startTime': startTime,
        'endTime': endTime,
        'latitude': latitude,
        'longitude': longitude,
        'radiusMeters': radiusMeters,
        'daysOfWeekJson': jsonEncode(daysOfWeek),
      };
}
