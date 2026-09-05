using MediatR;
using Mondabet.Report.Application.DTOs;
using Mondabet.Report.Application.Interfaces;
using Mondabet.Report.Application.Services;
using Mondabet.Shared.Domain;

namespace Mondabet.Report.Application.Queries.PayrollReport;

public class PayrollReportQueryHandler : IRequestHandler<PayrollReportQuery, Result<ReportFileDto>>
{
    private readonly IPayrollDataService _data;
    private readonly IReportGenerator _generator;

    public PayrollReportQueryHandler(IPayrollDataService data, IReportGenerator generator)
    {
        _data = data;
        _generator = generator;
    }

    public async Task<Result<ReportFileDto>> Handle(PayrollReportQuery request, CancellationToken ct)
    {
        var r = request.Request;
        if (r.Month is < 1 or > 12)
            return Error.Validation("Month must be between 1 and 12.");

        var from = new DateOnly(r.Year, r.Month, 1);
        var to = from.AddMonths(1).AddDays(-1);
        var fromDt = from.ToDateTime(TimeOnly.MinValue);
        var toDt = to.ToDateTime(TimeOnly.MaxValue);

        var employees = await _data.GetEmployeesAsync(ct);
        var departments = await _data.GetDepartmentsAsync(ct);
        var shifts = await _data.GetShiftsAsync(ct);
        var checkIns = await _data.GetCheckInsAsync(fromDt, toDt, ct);

        var departmentNamesById = departments.ToDictionary(d => d.Id, d => d.Name);
        var shiftsById = shifts.ToDictionary(s => s.Id);
        var checkInsByEmployee = checkIns
            .GroupBy(c => c.EmployeeId)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<PayrollCheckInInfo>)g.ToList());

        var rows = new List<PayrollSummaryDto>();
        foreach (var e in employees)
        {
            var departmentName = departmentNamesById.GetValueOrDefault(e.DepartmentId, "—");
            var employeeCheckIns = checkInsByEmployee.GetValueOrDefault(e.Id, Array.Empty<PayrollCheckInInfo>());

            var summary = PayrollCalculator.Calculate(e, departmentName, employeeCheckIns, shiftsById, from, to);
            if (summary is not null) rows.Add(summary);
        }

        var (content, contentType, extension) = r.Format switch
        {
            ReportFormat.Pdf => (
                _generator.GeneratePayrollPdf(rows, r.Year, r.Month),
                "application/pdf",
                "pdf"),
            _ => (
                _generator.GeneratePayrollExcel(rows, r.Year, r.Month),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "xlsx"),
        };

        var fileName = $"payroll_{r.Year:0000}_{r.Month:00}.{extension}";
        return new ReportFileDto(content, contentType, fileName);
    }
}
