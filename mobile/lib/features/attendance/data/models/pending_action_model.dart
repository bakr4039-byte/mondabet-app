import '../../domain/entities/pending_attendance_action.dart';

class PendingActionModel extends PendingAttendanceAction {
  const PendingActionModel({
    required super.id,
    required super.type,
    required super.queuedAt,
    super.shiftId,
    required super.lat,
    required super.lng,
    super.deviceId,
  });

  Map<String, dynamic> toJson() => {
        'id': id,
        'type': type.name,
        'queuedAt': queuedAt.toIso8601String(),
        'shiftId': shiftId,
        'lat': lat,
        'lng': lng,
        'deviceId': deviceId,
      };

  factory PendingActionModel.fromJson(Map<String, dynamic> json) => PendingActionModel(
        id: json['id'] as String,
        type: PendingActionType.values.byName(json['type'] as String),
        queuedAt: DateTime.parse(json['queuedAt'] as String),
        shiftId: json['shiftId'] as String?,
        lat: (json['lat'] as num).toDouble(),
        lng: (json['lng'] as num).toDouble(),
        deviceId: json['deviceId'] as String?,
      );
}
