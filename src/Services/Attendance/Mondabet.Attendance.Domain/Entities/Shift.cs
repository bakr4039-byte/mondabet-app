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

    // --- Split-shift support (e.g. a teacher's morning + afternoon block with a gap between
    // them). All additive/optional: a shift with IsSplitShift = false behaves exactly as
    // before, using StartTime/EndTime alone. StartTime/EndTime are kept as the overall
    // "first check-in .. last check-out" bounds even for a split shift, so nothing that only
    // knows about the plain start/end (e.g. reports) needs to change.
    public bool IsSplitShift { get; private set; } = false;
    public TimeOnly? FirstStartTime { get; private set; }
    public TimeOnly? FirstEndTime { get; private set; }
    public TimeOnly? SecondStartTime { get; private set; }
    public TimeOnly? SecondEndTime { get; private set; }

    private Shift() { }

    public static Shift Create(
        string name, TimeOnly start, TimeOnly end,
        double lat, double lng, int radiusMeters, string daysJson,
        int gracePeriodMinutes = 15, int windowStartMinutes = 60, int windowEndMinutes = 60,
        bool isSplitShift = false, TimeOnly? firstStartTime = null, TimeOnly? firstEndTime = null,
        TimeOnly? secondStartTime = null, TimeOnly? secondEndTime = null) => new()
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
        IsSplitShift = isSplitShift,
        FirstStartTime = firstStartTime,
        FirstEndTime = firstEndTime,
        SecondStartTime = secondStartTime,
        SecondEndTime = secondEndTime,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
    };

    public void Update(
        string name, TimeOnly start, TimeOnly end,
        double lat, double lng, int radiusMeters, string daysJson,
        int gracePeriodMinutes = 15, int windowStartMinutes = 60, int windowEndMinutes = 60,
        bool isSplitShift = false, TimeOnly? firstStartTime = null, TimeOnly? firstEndTime = null,
        TimeOnly? secondStartTime = null, TimeOnly? secondEndTime = null)
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
        IsSplitShift = isSplitShift;
        FirstStartTime = firstStartTime;
        FirstEndTime = firstEndTime;
        SecondStartTime = secondStartTime;
        SecondEndTime = secondEndTime;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// For a split shift, returns whichever of the two periods "now" is closer to (split at
    /// the midpoint of the gap between them), so check-in/check-out can apply that period's
    /// own grace/window rules. Non-split shifts (or a split shift missing any of the four
    /// period times) just return (StartTime, EndTime) unchanged - zero behavior change for
    /// every shift that isn't split.
    /// </summary>
    public (TimeOnly Start, TimeOnly End) GetActivePeriod(TimeOnly now)
    {
        if (!IsSplitShift || FirstStartTime is null || FirstEndTime is null
            || SecondStartTime is null || SecondEndTime is null)
            return (StartTime, EndTime);

        var gapStart = FirstEndTime.Value;
        var gapEnd = SecondStartTime.Value;
        var gap = gapEnd.ToTimeSpan() - gapStart.ToTimeSpan();
        var midpoint = gap > TimeSpan.Zero ? gapStart.Add(gap / 2) : gapStart;

        return now < midpoint
            ? (FirstStartTime.Value, FirstEndTime.Value)
            : (SecondStartTime.Value, SecondEndTime.Value);
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
