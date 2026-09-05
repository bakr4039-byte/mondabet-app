import '../repositories/attendance_repository.dart';

class GetPendingAttendanceCountUseCase {
  final AttendanceRepository repository;
  GetPendingAttendanceCountUseCase(this.repository);

  Future<int> call() => repository.getPendingCount();
}
