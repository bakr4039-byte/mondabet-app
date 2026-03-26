namespace Mondabet.Employee.Application.DTOs;

public record EmployeeDto(
    Guid Id,
    Guid UserId,
    string FullNameAr,
    string FullNameEn,
    string Iqama,
    DateOnly DateOfBirth,
    string JobTitle,
    string MobileNumber,
    string Email,
    Guid DepartmentId,
    Guid? ShiftId);

public record EmployeeCreateDto(
    string FullNameAr,
    string FullNameEn,
    string Iqama,
    DateOnly DateOfBirth,
    string JobTitle,
    string MobileNumber,
    string Email,
    Guid DepartmentId,
    Guid? ShiftId);

public record EmployeeUpdateDto(
    string FullNameAr,
    string FullNameEn,
    string JobTitle,
    string MobileNumber,
    string Email,
    Guid DepartmentId,
    Guid? ShiftId);

public record ImportResultDto(
    int Imported,
    int Skipped,
    IReadOnlyList<string> Errors);
