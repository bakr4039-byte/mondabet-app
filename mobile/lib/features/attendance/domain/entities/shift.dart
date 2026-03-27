class Shift {
  final String id;
  final String name;
  final String startTime;
  final String endTime;
  final double latitude;
  final double longitude;
  final int radiusMeters;
  final List<int> daysOfWeek;

  const Shift({
    required this.id,
    required this.name,
    required this.startTime,
    required this.endTime,
    required this.latitude,
    required this.longitude,
    required this.radiusMeters,
    required this.daysOfWeek,
  });
}
