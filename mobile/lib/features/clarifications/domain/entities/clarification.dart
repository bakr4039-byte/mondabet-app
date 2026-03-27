enum ClarificationStatus { pending, responded }

class ClarificationItem {
  final String id;
  final String question;
  final String? responseText;
  final ClarificationStatus status;
  final DateTime fromDate;
  final DateTime toDate;
  final DateTime createdAt;

  const ClarificationItem({
    required this.id,
    required this.question,
    this.responseText,
    required this.status,
    required this.fromDate,
    required this.toDate,
    required this.createdAt,
  });
}
