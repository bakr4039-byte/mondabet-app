namespace Mondabet.Attendance.Application.DTOs;

public record ShiftDto(
    Guid Id,
    string Name,
    TimeOnly StartTime,
    TimeOnly EndTime,
    double Latitude,
    double Longitude,
    int RadiusMeters,
    string DaysOfWeekJson,
    int GracePeriodMinutes,
    int WindowStartMinutes,
    int WindowEndMinutes);

public record ShiftCreateDto(
    string Name,
    TimeOnly StartTime,
    TimeOnly EndTime,
    double Latitude,
    double Longitude,
    int RadiusMeters,
    string DaysOfWeekJson,
    int GracePeriodMinutes = 15,
    int WindowStartMinutes = 60,
    int WindowEndMinutes = 60);

public record ShiftUpdateDto(
    string Name,
    TimeOnly StartTime,
    TimeOnly EndTime,
    double Latitude,
    double Longitude,
    int RadiusMeters,
    string DaysOfWeekJson,
    int GracePeriodMinutes = 15,
    int WindowStartMinutes = 60,
    int WindowEndMinutes = 60);
