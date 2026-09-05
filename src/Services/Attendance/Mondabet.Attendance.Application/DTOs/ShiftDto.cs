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
    int WindowEndMinutes,
    bool IsSplitShift = false,
    TimeOnly? FirstStartTime = null,
    TimeOnly? FirstEndTime = null,
    TimeOnly? SecondStartTime = null,
    TimeOnly? SecondEndTime = null);

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
    int WindowEndMinutes = 60,
    bool IsSplitShift = false,
    TimeOnly? FirstStartTime = null,
    TimeOnly? FirstEndTime = null,
    TimeOnly? SecondStartTime = null,
    TimeOnly? SecondEndTime = null);

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
    int WindowEndMinutes = 60,
    bool IsSplitShift = false,
    TimeOnly? FirstStartTime = null,
    TimeOnly? FirstEndTime = null,
    TimeOnly? SecondStartTime = null,
    TimeOnly? SecondEndTime = null);
