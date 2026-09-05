import 'dart:convert';

import 'package:hive_flutter/hive_flutter.dart';

import '../models/pending_action_model.dart';
import '../models/shift_model.dart';

/// Local (Hive) cache for offline attendance: the last-known shift (so the
/// check-in screen still has geofence data with no connection) and a queue of
/// check-in/check-out actions performed while offline, flushed by
/// AttendanceRepositoryImpl.syncPendingActions() once connectivity returns.
///
/// Stored as plain JSON strings (no generated TypeAdapter/build_runner step),
/// matching the existing TenantConfig Hive usage in this project.
abstract class AttendanceLocalDataSource {
  Future<void> cacheShift(ShiftModel shift);
  Future<ShiftModel?> getCachedShift();

  Future<void> enqueuePendingAction(PendingActionModel action);
  Future<List<PendingActionModel>> getPendingActions();
  Future<void> removePendingAction(String id);
}

class AttendanceLocalDataSourceImpl implements AttendanceLocalDataSource {
  static const _shiftBoxName = 'attendance_shift_cache';
  static const _queueBoxName = 'attendance_pending_queue';
  static const _shiftKey = 'current';

  @override
  Future<void> cacheShift(ShiftModel shift) async {
    final box = await Hive.openBox<String>(_shiftBoxName);
    await box.put(_shiftKey, jsonEncode(shift.toJson()));
  }

  @override
  Future<ShiftModel?> getCachedShift() async {
    final box = await Hive.openBox<String>(_shiftBoxName);
    final raw = box.get(_shiftKey);
    if (raw == null) return null;
    return ShiftModel.fromJson(jsonDecode(raw) as Map<String, dynamic>);
  }

  @override
  Future<void> enqueuePendingAction(PendingActionModel action) async {
    final box = await Hive.openBox<String>(_queueBoxName);
    await box.put(action.id, jsonEncode(action.toJson()));
  }

  @override
  Future<List<PendingActionModel>> getPendingActions() async {
    final box = await Hive.openBox<String>(_queueBoxName);
    final actions = box.values
        .map((raw) => PendingActionModel.fromJson(jsonDecode(raw) as Map<String, dynamic>))
        .toList()
      ..sort((a, b) => a.queuedAt.compareTo(b.queuedAt));
    return actions;
  }

  @override
  Future<void> removePendingAction(String id) async {
    final box = await Hive.openBox<String>(_queueBoxName);
    await box.delete(id);
  }
}
