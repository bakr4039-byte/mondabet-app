import '../../domain/entities/leave_request.dart';

class LeaveModel extends LeaveRequest {
  const LeaveModel({
    required super.id,
    required super.type,
    required super.status,
    required super.fromDate,
    required super.toDate,
    super.reason,
    super.attachmentUrl,
    required super.createdAt,
  });

  factory LeaveModel.fromJson(Map<String, dynamic> json) => LeaveModel(
        id: json['id'] as String,
        type: _parseType(json['type'] as int),
        status: _parseStatus(json['status'] as int),
        fromDate: DateTime.parse(json['fromDate'] as String),
        toDate: DateTime.parse(json['toDate'] as String),
        reason: json['reason'] as String?,
        attachmentUrl: json['attachmentUrl'] as String?,
        createdAt: DateTime.parse(json['createdAt'] as String),
      );

  static LeaveType _parseType(int v) => switch (v) {
        1 => LeaveType.vacation,
        2 => LeaveType.permission,
        _ => LeaveType.excuse,
      };

  static LeaveStatus _parseStatus(int v) => switch (v) {
        2 => LeaveStatus.approved,
        3 => LeaveStatus.rejected,
        _ => LeaveStatus.pending,
      };
}
