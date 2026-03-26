namespace Mondabet.Employee.Application.Interfaces;

public interface IExcelImportService
{
    Task<IReadOnlyList<EmployeeExcelRow>> ParseEmployeeExcelAsync(
        Stream stream, CancellationToken ct = default);
}

public record EmployeeExcelRow(
    int RowNumber,
    string FullNameAr,
    string FullNameEn,
    string Iqama,
    string DateOfBirth,
    string JobTitle,
    string MobileNumber,
    string Email,
    Guid DepartmentId,
    Guid? ShiftId);
