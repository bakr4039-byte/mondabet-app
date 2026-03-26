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

    private Shift() { }

    public static Shift Create(
        string name, TimeOnly start, TimeOnly end,
        double lat, double lng, int radiusMeters, string daysJson) => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        StartTime = start,
        EndTime = end,
        Latitude = lat,
        Longitude = lng,
        RadiusMeters = radiusMeters,
        DaysOfWeekJson = daysJson,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
    };

    public void Update(
        string name, TimeOnly start, TimeOnly end,
        double lat, double lng, int radiusMeters, string daysJson)
    {
        Name = name;
        StartTime = start;
        EndTime = end;
        Latitude = lat;
        Longitude = lng;
        RadiusMeters = radiusMeters;
        DaysOfWeekJson = daysJson;
        UpdatedAt = DateTime.UtcNow;
    }
}
