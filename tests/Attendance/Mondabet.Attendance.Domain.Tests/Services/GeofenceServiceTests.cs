using FluentAssertions;
using Mondabet.Attendance.Domain.Services;
using Xunit;

namespace Mondabet.Attendance.Domain.Tests.Services;

public class GeofenceServiceTests
{
    // Riyadh city center coordinates for reference
    private const double RiyadhLat = 24.7136;
    private const double RiyadhLng = 46.6753;

    [Fact]
    public void IsWithinGeofence_WhenEmployeeAtShiftLocation_ReturnsTrue()
    {
        // Arrange: employee exactly at shift location
        var result = GeofenceService.IsWithinGeofence(
            RiyadhLat, RiyadhLng,
            RiyadhLat, RiyadhLng,
            radiusMeters: 100);

        result.Should().BeTrue();
    }

    [Fact]
    public void IsWithinGeofence_WhenEmployeeWithinRadius_ReturnsTrue()
    {
        // Arrange: ~50m north (roughly 0.00045 degrees lat)
        var employeeLat = RiyadhLat + 0.00045;
        var result = GeofenceService.IsWithinGeofence(
            employeeLat, RiyadhLng,
            RiyadhLat, RiyadhLng,
            radiusMeters: 100);

        result.Should().BeTrue();
    }

    [Fact]
    public void IsWithinGeofence_WhenEmployeeOutsideRadius_ReturnsFalse()
    {
        // Arrange: ~500m north
        var employeeLat = RiyadhLat + 0.0045;
        var result = GeofenceService.IsWithinGeofence(
            employeeLat, RiyadhLng,
            RiyadhLat, RiyadhLng,
            radiusMeters: 100);

        result.Should().BeFalse();
    }

    [Fact]
    public void IsWithinGeofence_AtExactRadius_ReturnsTrue()
    {
        // 100m east (approx 0.001 degrees lng at equator is ~111m, adjust for lat)
        // At lat ~24.7, 1 degree lng ≈ 101,100m → 100m ≈ 0.000989 degrees
        var employeeLng = RiyadhLng + 0.000989;
        var result = GeofenceService.IsWithinGeofence(
            RiyadhLat, employeeLng,
            RiyadhLat, RiyadhLng,
            radiusMeters: 100);

        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(24.7136, 46.6753, 24.7136, 46.6753, 0)]       // same point = 0m
    [InlineData(0, 0, 0, 1, 111_195)]                          // 1 degree lng at equator ≈ 111195m
    public void HaversineDistance_KnownValues(
        double lat1, double lng1, double lat2, double lng2, int expectedMeters)
    {
        var distance = GeofenceService.HaversineDistance(lat1, lng1, lat2, lng2);
        distance.Should().BeApproximately(expectedMeters, precision: 200);
    }
}
