using Mondabet.Shared.Domain;

namespace Mondabet.Attendance.Domain.Entities;

public class Shift : BaseEntity
{
    public string Name { get; private set; } = default!;
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public int RadiusMeters { get; private set; }
    public string DaysOfWeekJson { get; private set; } = "[]";

    /// <summary>Minutes after StartTime a check-in is still considered on-time.</summary>
    public int GracePeriodMinutes { get; private set; } = 15;

    /// <summary>How many minutes before StartTime a check-in is accepted at all.</summary>
    public int WindowStartMinutes { get; private set; } = 60;

    /// <summary>How many minutes after EndTime a check-in/out is still accepted.</summary>
    public int WindowEndMinutes { get; private set; } = 60;

    private Shift() { }

    public static Shift Create(
        string name, TimeOnly start, TimeOnly end,
        double lat, double lng, int radiusMeters, string daysJson,
        int gracePeriodMinutes = 15, int windowStartMinutes = 60, int windowEndMinutes = 60) => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        StartTime = start,
        EndTime = end,
        Latitude = lat,
        Longitude = lng,
        RadiusMeters = radiusMeters,
        DaysOfWeekJson = daysJson,
        GracePeriodMinutes = gracePeriodMinutes,
        WindowStartMinutes = windowStartMinutes,
        WindowEndMinutes = windowEndMinutes,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
    };

    public void Update(
        string name, TimeOnly start, TimeOnly end,
        double lat, double lng, int radiusMeters, string daysJson,
        int gracePeriodMinutes = 15, int windowStartMinutes = 60, int windowEndMinutes = 60)
    {
        Name = name;
        StartTime = start;
        EndTime = end;
        Latitude = lat;
        Longitude = lng;
        RadiusMeters = radiusMeters;
        DaysOfWeekJson = daysJson;
        GracePeriodMinutes = gracePeriodMinutes;
        WindowStartMinutes = windowStartMinutes;
        WindowEndMinutes = windowEndMinutes;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>True if this shift is scheduled to run on the given day (0 = Sunday .. 6 = Saturday).</summary>
    public bool IsScheduledOn(DayOfWeek day)
    {
        // DaysOfWeekJson stores a JSON int array, e.g. "[0,1,2,3,4]". Empty/invalid => treated as "every day".
        try
        {
            var days = System.Text.Json.JsonSerializer.Deserialize<int[]>(DaysOfWeekJson);
            if (days is null || days.Length == 0) return true;
            var todayIndex = (int)day; // System.DayOfWeek: Sunday = 0 .. Saturday = 6, matches the prototype's convention.
            return Array.IndexOf(days, todayIndex) >= 0;
        }
        catch
        {
            return true;
        }
    }
}
