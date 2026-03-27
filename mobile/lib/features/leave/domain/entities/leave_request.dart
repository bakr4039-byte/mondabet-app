enum LeaveType { vacation, permission, excuse }
enum LeaveStatus { pending, approved, rejected }

class LeaveRequest {
  final String id;
  final LeaveType type;
  final LeaveStatus status;
  final DateTime fromDate;
  final DateTime toDate;
  final String? reason;
  final String? attachmentUrl;
  final DateTime createdAt;

  const LeaveRequest({
    required this.id,
    required this.type,
    required this.status,
    required this.fromDate,
    required this.toDate,
    this.reason,
    this.attachmentUrl,
    required this.createdAt,
  });
}
