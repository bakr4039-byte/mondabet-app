using ClosedXML.Excel;
using Mondabet.Report.Application.DTOs;
using Mondabet.Report.Application.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Mondabet.Report.Infrastructure.Services;

public class ReportGenerator : IReportGenerator
{
    static ReportGenerator()
    {
        // QuestPDF community license (free for revenue < $1M USD)
        QuestPDF.Settings.License = LicenseType.Community;
    }

    // ──────────────────────────────────────────────────────────
    // Attendance PDF
    // ──────────────────────────────────────────────────────────
    public byte[] GenerateAttendancePdf(
        IReadOnlyList<AttendanceRow> rows, DateTime from, DateTime to)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1, Unit.Centimetre);

                page.Header().Text($"Attendance Report: {from:dd/MM/yyyy} – {to:dd/MM/yyyy}")
                    .FontSize(14).Bold();

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(3); // Employee
                        cols.RelativeColumn(2); // Iqama
                        cols.RelativeColumn(3); // Check-in
                        cols.RelativeColumn(3); // Check-out
                        cols.RelativeColumn(2); // Geofence
                        cols.RelativeColumn(2); // Shift
                    });

                    // Header row
                    static IContainer HeaderCell(IContainer c) =>
                        c.Background(Colors.Grey.Lighten2).Padding(5);

                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderCell).Text("Employee");
                        header.Cell().Element(HeaderCell).Text("Iqama");
                        header.Cell().Element(HeaderCell).Text("Check In");
                        header.Cell().Element(HeaderCell).Text("Check Out");
                        header.Cell().Element(HeaderCell).Text("Geofence");
                        header.Cell().Element(HeaderCell).Text("Shift");
                    });

                    foreach (var r in rows)
                    {
                        table.Cell().Padding(4).Text(r.EmployeeName);
                        table.Cell().Padding(4).Text(r.Iqama);
                        table.Cell().Padding(4).Text(r.CheckInTime.ToString("dd/MM/yyyy HH:mm"));
                        table.Cell().Padding(4).Text(r.CheckOutTime?.ToString("HH:mm") ?? "—");
                        table.Cell().Padding(4).Text(r.IsWithinGeofence ? "✓" : "✗");
                        table.Cell().Padding(4).Text(r.ShiftName);
                    }
                });

                page.Footer().AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" of ");
                        x.TotalPages();
                    });
            });
        }).GeneratePdf();
    }

    // ──────────────────────────────────────────────────────────
    // Attendance Excel
    // ──────────────────────────────────────────────────────────
    public byte[] GenerateAttendanceExcel(
        IReadOnlyList<AttendanceRow> rows, DateTime from, DateTime to)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Attendance");

        ws.Cell(1, 1).Value = "Employee";
        ws.Cell(1, 2).Value = "Iqama";
        ws.Cell(1, 3).Value = "Check In";
        ws.Cell(1, 4).Value = "Check Out";
        ws.Cell(1, 5).Value = "Geofence";
        ws.Cell(1, 6).Value = "Shift";

        var headerRow = ws.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

        var rowIdx = 2;
        foreach (var r in rows)
        {
            ws.Cell(rowIdx, 1).Value = r.EmployeeName;
            ws.Cell(rowIdx, 2).Value = r.Iqama;
            ws.Cell(rowIdx, 3).Value = r.CheckInTime;
            ws.Cell(rowIdx, 3).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
            ws.Cell(rowIdx, 4).Value = r.CheckOutTime?.ToString("HH:mm") ?? "";
            ws.Cell(rowIdx, 5).Value = r.IsWithinGeofence ? "Yes" : "No";
            ws.Cell(rowIdx, 6).Value = r.ShiftName;
            rowIdx++;
        }

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    // ──────────────────────────────────────────────────────────
    // Company PDF
    // ──────────────────────────────────────────────────────────
    public byte[] GenerateCompanyPdf(IReadOnlyList<TenantSummaryRow> rows)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.Header().Text("Company Summary Report").FontSize(14).Bold();

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(4);
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(3);
                    });

                    static IContainer H(IContainer c) =>
                        c.Background(Colors.Grey.Lighten2).Padding(5);

                    table.Header(header =>
                    {
                        header.Cell().Element(H).Text("Code");
                        header.Cell().Element(H).Text("Company");
                        header.Cell().Element(H).Text("Employees");
                        header.Cell().Element(H).Text("Last Activity");
                    });

                    foreach (var r in rows)
                    {
                        table.Cell().Padding(4).Text(r.TenantCode);
                        table.Cell().Padding(4).Text(r.CompanyName);
                        table.Cell().Padding(4).Text(r.EmployeeCount.ToString());
                        table.Cell().Padding(4).Text(r.LastActivity?.ToString("dd/MM/yyyy") ?? "—");
                    }
                });
            });
        }).GeneratePdf();
    }

    // ──────────────────────────────────────────────────────────
    // Company Excel
    // ──────────────────────────────────────────────────────────
    public byte[] GenerateCompanyExcel(IReadOnlyList<TenantSummaryRow> rows)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Companies");

        ws.Cell(1, 1).Value = "Code";
        ws.Cell(1, 2).Value = "Company";
        ws.Cell(1, 3).Value = "Employees";
        ws.Cell(1, 4).Value = "Last Activity";

        ws.Row(1).Style.Font.Bold = true;
        ws.Row(1).Style.Fill.BackgroundColor = XLColor.LightGray;

        var rowIdx = 2;
        foreach (var r in rows)
        {
            ws.Cell(rowIdx, 1).Value = r.TenantCode;
            ws.Cell(rowIdx, 2).Value = r.CompanyName;
            ws.Cell(rowIdx, 3).Value = r.EmployeeCount;
            ws.Cell(rowIdx, 4).Value = r.LastActivity?.ToString("dd/MM/yyyy") ?? "";
            rowIdx++;
        }

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    // ──────────────────────────────────────────────────────────
    // Payroll PDF
    // ──────────────────────────────────────────────────────────
    public byte[] GeneratePayrollPdf(IReadOnlyList<PayrollSummaryDto> rows, int year, int month)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1, Unit.Centimetre);
                page.Header().Text($"Payroll Report: {year:0000}-{month:00}").FontSize(14).Bold();

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(3); // Employee
                        cols.RelativeColumn(2); // Department
                        cols.RelativeColumn(2); // Base salary
                        cols.RelativeColumn(1); // Present
                        cols.RelativeColumn(1); // Late
                        cols.RelativeColumn(2); // Overtime pay
                        cols.RelativeColumn(2); // Late deduction
                        cols.RelativeColumn(2); // Net
                    });

                    static IContainer H(IContainer c) =>
                        c.Background(Colors.Grey.Lighten2).Padding(5);

                    table.Header(header =>
                    {
                        header.Cell().Element(H).Text("Employee");
                        header.Cell().Element(H).Text("Department");
                        header.Cell().Element(H).Text("Base Salary");
                        header.Cell().Element(H).Text("Present");
                        header.Cell().Element(H).Text("Late");
                        header.Cell().Element(H).Text("Overtime Pay");
                        header.Cell().Element(H).Text("Late Deduction");
                        header.Cell().Element(H).Text("Net Payable");
                    });

                    foreach (var r in rows)
                    {
                        table.Cell().Padding(4).Text(r.EmployeeName);
                        table.Cell().Padding(4).Text(r.DepartmentName);
                        table.Cell().Padding(4).Text($"{r.BaseSalary:N2} {r.Currency}");
                        table.Cell().Padding(4).Text($"{r.PresentDays}/{r.WorkingDays}");
                        table.Cell().Padding(4).Text(r.LateDays.ToString());
                        table.Cell().Padding(4).Text($"{r.OvertimePay:N2}");
                        table.Cell().Padding(4).Text($"{r.LateDeduction:N2}");
                        table.Cell().Padding(4).Text($"{r.NetPayableSalary:N2} {r.Currency}");
                    }
                });

                page.Footer().AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" of ");
                        x.TotalPages();
                    });
            });
        }).GeneratePdf();
    }

    // ──────────────────────────────────────────────────────────
    // Payroll Excel
    // ──────────────────────────────────────────────────────────
    public byte[] GeneratePayrollExcel(IReadOnlyList<PayrollSummaryDto> rows, int year, int month)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add($"Payroll {year:0000}-{month:00}");

        string[] headers =
        {
            "Employee", "Employee (AR)", "Employee #", "Department", "Base Salary",
            "Working Days", "Present Days", "Late Days", "Late Minutes",
            "Work Hours", "Overtime Hours", "Overtime Pay", "Late Deduction",
            "Net Payable", "Currency",
        };
        for (var i = 0; i < headers.Length; i++) ws.Cell(1, i + 1).Value = headers[i];

        ws.Row(1).Style.Font.Bold = true;
        ws.Row(1).Style.Fill.BackgroundColor = XLColor.LightGray;

        var rowIdx = 2;
        foreach (var r in rows)
        {
            ws.Cell(rowIdx, 1).Value = r.EmployeeName;
            ws.Cell(rowIdx, 2).Value = r.EmployeeNameAr;
            ws.Cell(rowIdx, 3).Value = r.EmployeeNumber ?? "";
            ws.Cell(rowIdx, 4).Value = r.DepartmentName;
            ws.Cell(rowIdx, 5).Value = r.BaseSalary;
            ws.Cell(rowIdx, 6).Value = r.WorkingDays;
            ws.Cell(rowIdx, 7).Value = r.PresentDays;
            ws.Cell(rowIdx, 8).Value = r.LateDays;
            ws.Cell(rowIdx, 9).Value = r.TotalLateMinutes;
            ws.Cell(rowIdx, 10).Value = r.TotalWorkHours;
            ws.Cell(rowIdx, 11).Value = r.TotalOvertimeHours;
            ws.Cell(rowIdx, 12).Value = r.OvertimePay;
            ws.Cell(rowIdx, 13).Value = r.LateDeduction;
            ws.Cell(rowIdx, 14).Value = r.NetPayableSalary;
            ws.Cell(rowIdx, 15).Value = r.Currency;
            rowIdx++;
        }

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }
}
