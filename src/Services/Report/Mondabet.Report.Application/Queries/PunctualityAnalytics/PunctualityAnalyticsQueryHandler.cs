using MediatR;
using Mondabet.Report.Application.DTOs;
using Mondabet.Report.Application.Interfaces;
using Mondabet.Shared.Domain;

namespace Mondabet.Report.Application.Queries.PunctualityAnalytics;

/// <summary>
/// Surfaces the Employee entity's dead PunctualityScore/ConsecutiveOnTimeDays/GamificationBadge
/// fields (settable in code, never actually computed or displayed anywhere). Rather than adding
/// a background job that writes those fields back onto Employee, this recomputes the same
/// metrics on demand from the check-in history IPayrollDataService already exposes for the
/// payroll report - so this is a pure read, no new writes/migrations/cross-service calls needed
/// beyond what already exists.
/// </summary>
public class PunctualityAnalyticsQueryHandler
    : IRequestHandler<PunctualityAnalyticsQuery, Result<IReadOnlyList<EmployeePunctualityDto>>>
{
    private static readonly PayrollCheckStatus[] OnTimeLikeStatuses =
    {
        PayrollCheckStatus.OnTime,
        PayrollCheckStatus.Normal,
        PayrollCheckStatus.FieldMission,
        PayrollCheckStatus.EarlyDeparture,
    };

    private const double GoldScoreThreshold = 95;
    private const int GoldStreakThreshold = 10;
    private const double SilverScoreThreshold = 85;
    private const double BronzeScoreThreshold = 70;

    private readonly IPayrollDataService _data;

    public PunctualityAnalyticsQueryHandler(IPayrollDataService data)
    {
        _data = data;
    }

    public async Task<Result<IReadOnlyList<EmployeePunctualityDto>>> Handle(
        PunctualityAnalyticsQuery request, CancellationToken ct)
    {
        if (request.From > request.To)
            return Error.Validation("'From' date must not be after 'To' date.");

        var employees = await _data.GetEmployeesAsync(ct);
        var checkIns = await _data.GetCheckInsAsync(request.From, request.To, ct);

        var checkInsByEmployee = checkIns
            .GroupBy(c => c.EmployeeId)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<PayrollCheckInInfo>)g.ToList());

        var rows = new List<EmployeePunctualityDto>();
        foreach (var employee in employees)
        {
            if (!checkInsByEmployee.TryGetValue(employee.Id, out var employeeCheckIns) ||
                employeeCheckIns.Count == 0)
            {
                continue; // no attendance in range - nothing to score, skip rather than show a false 0%
            }

            // Counted check-ins are ones that reflect an actual attendance outcome; rejected
            // punches (out-of-fence/out-of-shift) and "not scheduled today" carry no punctuality
            // signal, so they're excluded from both the denominator and the streak.
            var counted = employeeCheckIns
                .Where(c => c.CheckInStatus != PayrollCheckStatus.OutOfFenceRejected &&
                            c.CheckInStatus != PayrollCheckStatus.OutOfShiftRejected &&
                            c.CheckInStatus != PayrollCheckStatus.NotScheduledToday)
                .OrderBy(c => c.CheckInTime)
                .ToList();

            if (counted.Count == 0) continue;

            var onTimeCount = counted.Count(c => OnTimeLikeStatuses.Contains(c.CheckInStatus));
            var lateCount = counted.Count(c => c.CheckInStatus == PayrollCheckStatus.Late);
            var totalCheckIns = counted.Count;
            var score = totalCheckIns == 0 ? 0 : (double)onTimeCount / totalCheckIns * 100;

            // Walk day-by-day, most recent first, treating a day as "on time" only if none of
            // that day's counted check-ins was late.
            var byDay = counted
                .GroupBy(c => c.CheckInTime.Date)
                .OrderByDescending(g => g.Key)
                .ToList();

            var streak = 0;
            foreach (var day in byDay)
            {
                var dayIsOnTime = day.All(c => c.CheckInStatus != PayrollCheckStatus.Late);
                if (!dayIsOnTime) break;
                streak++;
            }

            var badge = (score, streak) switch
            {
                (>= GoldScoreThreshold, >= GoldStreakThreshold) => "Gold",
                ( >= SilverScoreThreshold, _) => "Silver",
                ( >= BronzeScoreThreshold, _) => "Bronze",
                _ => "None",
            };

            rows.Add(new EmployeePunctualityDto(
                employee.Id,
                employee.FullNameEn,
                employee.FullNameAr,
                totalCheckIns,
                onTimeCount,
                lateCount,
                Math.Round(score, 1),
                streak,
                badge));
        }

        return rows
            .OrderByDescending(r => r.PunctualityScore)
            .ThenByDescending(r => r.ConsecutiveOnTimeDays)
            .ToList();
    }
}
