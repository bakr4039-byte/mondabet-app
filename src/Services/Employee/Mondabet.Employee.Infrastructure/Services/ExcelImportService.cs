using ClosedXML.Excel;
using Mondabet.Employee.Application.Interfaces;

namespace Mondabet.Employee.Infrastructure.Services;

public class ExcelImportService : IExcelImportService
{
    // Expected column order: FullNameAr, FullNameEn, Iqama, DateOfBirth,
    //                        JobTitle, MobileNumber, Email, DepartmentId, ShiftId
    public Task<IReadOnlyList<EmployeeExcelRow>> ParseEmployeeExcelAsync(
        Stream stream, CancellationToken ct = default)
    {
        using var wb = new XLWorkbook(stream);
        var ws = wb.Worksheet(1);
        var rows = new List<EmployeeExcelRow>();

        foreach (var row in ws.RowsUsed().Skip(1)) // skip header
        {
            var rowNum = row.RowNumber();
            rows.Add(new EmployeeExcelRow(
                RowNumber: rowNum,
                FullNameAr: row.Cell(1).GetString(),
                FullNameEn: row.Cell(2).GetString(),
                Iqama: row.Cell(3).GetString(),
                DateOfBirth: row.Cell(4).GetString(),
                JobTitle: row.Cell(5).GetString(),
                MobileNumber: row.Cell(6).GetString(),
                Email: row.Cell(7).GetString(),
                DepartmentId: Guid.TryParse(row.Cell(8).GetString(), out var deptId)
                    ? deptId : Guid.Empty,
                ShiftId: Guid.TryParse(row.Cell(9).GetString(), out var shiftId)
                    ? shiftId : null));
        }

        return Task.FromResult<IReadOnlyList<EmployeeExcelRow>>(rows);
    }
}
