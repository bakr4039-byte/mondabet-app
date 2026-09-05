using Mondabet.Report.Application.DTOs;
using Mondabet.Report.Application.Interfaces;

namespace Mondabet.Report.Application.Services;

/// <summary>
/// Pure computation, deliberately kept separate from the HTTP-fetching in
/// Mondabet.Report.Infrastructure.Services.PayrollDataService — this is the part a company's
/// actual payroll policy will need to tune, so it's isolated here rather than tangled up with
/// network calls.
///
/// IMPORTANT — the formulas below are reasonable V1 defaults, not confirmed company policy:
///   - overtime pay = overtime hours x hourly rate x 1.5 (a common "time and a half" convention)
///   - late deduction = late minutes x (hourly rate / 60), i.e. straight pro-rata
///   - an employee's hourly rate, when not set explicitly, is derived from BaseSalary / 240
///     (a common "30 days x 8 hours" assumption)
/// None of this accounts for GOSI/social-insurance deductions, allowances, or Saudi labor-law
/// specifics — this is meant as a starting point to refine once real payroll rules are confirmed.
/// </summary>
public static class PayrollCalculator
{
    public const string DefaultCurrency = "SAR";
    private const int AssumedMonthlyHours = 240;
    private const decimal OvertimeMultiplier = 1.5m;

    private static readonly PayrollCheckStatus[] ValidPresenceStatuses =
    {
        PayrollCheckStatus.OnTime,
        PayrollCheckStatus.Late,
        PayrollCheckStatus.Normal,
        PayrollCheckStatus.FieldMission,
        PayrollCheckStatus.EarlyDeparture,
    };

    /// <summary>Returns null when the employee has neither BaseSalary nor HourlyRate set — there's
    /// nothing to payroll them on yet, so the caller should skip them rather than show a zero.</summary>
    public static PayrollSummaryDto? Calculate(
        PayrollEmployeeInfo employee,
        string departmentName,
        IReadOnlyList<PayrollCheckInInfo> employeeCheckIns,
        IReadOnlyDictionary<Guid, PayrollShiftInfo> shiftsById,
        DateOnly from,
        DateOnly to)
    {
        if (employee.BaseSalary is null && employee.HourlyRate is null) return null;

        var baseSalary = employee.BaseSalary ?? 0m;
        var hourlyRate = employee.HourlyRate ?? (baseSalary / AssumedMonthlyHours);

        var validCheckIns = employeeCheckIns.Where(c => ValidPresenceStatuses.Contains(c.CheckInStatus)).ToList();

        var presentDays = validCheckIns.Select(c => c.CheckInTime.Date).Distinct().Count();
        var lateCheckIns = validCheckIns.Where(c => c.CheckInStatus == PayrollCheckStatus.Late).ToList();
        var lateDays = lateCheckIns.Select(c => c.CheckInTime.Date).Distinct().Count();

        var totalLateMinutes = 0;
        foreach (var c in lateCheckIns)
        {
            if (!shiftsById.TryGetValue(c.ShiftId, out var shift)) continue;
            var lateBy = c.CheckInTime.TimeOfDay - shift.StartTime.ToTimeSpan();
            if (lateBy.TotalMinutes > 0) totalLateMinutes += (int)lateBy.TotalMinutes;
        }

        double totalWorkHours = 0, totalOvertimeHours = 0;
        foreach (var c in validCheckIns)
        {
            if (c.CheckOutTime is null) continue;
            var workedHours = (c.CheckOutTime.Value - c.CheckInTime).TotalHours;
            if (workedHours <= 0) continue;
            totalWorkHours += workedHours;

            if (shiftsById.TryGetValue(c.ShiftId, out var shift))
            {
                var scheduledHours = (shift.EndTime.ToTimeSpan() - shift.StartTime.ToTimeSpan()).TotalHours;
                if (scheduledHours <= 0) scheduledHours += 24; // shift wraps past midnight
                if (workedHours > scheduledHours) totalOvertimeHours += workedHours - scheduledHours;
            }
        }

        var overtimePay = (decimal)totalOvertimeHours * hourlyRate * OvertimeMultiplier;
        var lateDeduction = totalLateMinutes / 60m * hourlyRate;
        var netPayableSalary = baseSalary + overtimePay - lateDeduction;

        var workingDays = CountWorkingDays(employee.ShiftId, shiftsById, from, to);

        return new PayrollSummaryDto(
            employee.Id, employee.FullNameEn, employee.FullNameAr, employee.EmployeeNumber,
            departmentName, baseSalary, workingDays, presentDays, lateDays,
            totalLateMinutes, Math.Round(totalWorkHours, 2), Math.Round(totalOvertimeHours, 2),
            Math.Round(overtimePay, 2), Math.Round(lateDeduction, 2), Math.Round(netPayableSalary, 2),
            DefaultCurrency);
    }

    /// <summary>Number of days in [from, to] the employee's shift is actually scheduled on
    /// (parsing the same DaysOfWeekJson format Attendance's Shift.IsScheduledOn uses). Falls
    /// back to "every day in the range" when the employee has no shift or the shift has no
    /// day restriction — matching that same method's own fallback.</summary>
    private static int CountWorkingDays(
        Guid? shiftId, IReadOnlyDictionary<Guid, PayrollShiftInfo> shiftsById, DateOnly from, DateOnly to)
    {
        int[]? scheduledDays = null;
        if (shiftId.HasValue && shiftsById.TryGetValue(shiftId.Value, out var shift))
        {
            try
            {
                scheduledDays = System.Text.Json.JsonSerializer.Deserialize<int[]>(shift.DaysOfWeekJson);
            }
            catch { /* fall through to "every day" */ }
        }

        var count = 0;
        for (var d = from; d <= to; d = d.AddDays(1))
        {
            if (scheduledDays is null || scheduledDays.Length == 0 || scheduledDays.Contains((int)d.DayOfWeek))
                count++;
        }
        return count;
    }
}
