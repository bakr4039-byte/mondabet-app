namespace Mondabet.Attendance.Application.DTOs;

public record ShiftDto(
    Guid Id,
    string Name,
    TimeOnly StartTime,
    TimeOnly EndTime,
    double Latitude,
    double Longitude,
    int RadiusMeters,
    string DaysOfWeekJson);

public record ShiftCreateDto(
    string Name,
    TimeOnly StartTime,
    TimeOnly EndTime,
    double Latitude,
    double Longitude,
    int RadiusMeters,
    string DaysOfWeekJson);

public record ShiftUpdateDto(
    string Name,
    TimeOnly StartTime,
    TimeOnly EndTime,
    double Latitude,
    double Longitude,
    int RadiusMeters,
    string DaysOfWeekJson);
