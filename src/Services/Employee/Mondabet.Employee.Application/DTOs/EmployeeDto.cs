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
    Guid? ShiftId,
    bool IsActive,
    string? EmployeeNumber,
    int? FingerprintId,
    string? PinCode,
    DateOnly? IqamaExpiryDate,
    DateOnly? ContractExpiryDate,
    DateOnly? HealthCertExpiryDate,
    DateOnly? MedicalInsuranceExpiryDate,
    DateOnly? DrivingLicenseExpiryDate,
    double? PunctualityScore,
    int? ConsecutiveOnTimeDays,
    string? GamificationBadge);

public record EmployeeCreateDto(
    string FullNameAr,
    string FullNameEn,
    string Iqama,
    DateOnly DateOfBirth,
    string JobTitle,
    string MobileNumber,
    string Email,
    Guid DepartmentId,
    Guid? ShiftId,
    string? EmployeeNumber = null,
    int? FingerprintId = null,
    string? PinCode = null,
    DateOnly? IqamaExpiryDate = null,
    DateOnly? ContractExpiryDate = null,
    DateOnly? HealthCertExpiryDate = null,
    DateOnly? MedicalInsuranceExpiryDate = null,
    DateOnly? DrivingLicenseExpiryDate = null);

public record EmployeeUpdateDto(
    string FullNameAr,
    string FullNameEn,
    string JobTitle,
    string MobileNumber,
    string Email,
    Guid DepartmentId,
    Guid? ShiftId,
    string? EmployeeNumber = null,
    int? FingerprintId = null,
    string? PinCode = null,
    DateOnly? IqamaExpiryDate = null,
    DateOnly? ContractExpiryDate = null,
    DateOnly? HealthCertExpiryDate = null,
    DateOnly? MedicalInsuranceExpiryDate = null,
    DateOnly? DrivingLicenseExpiryDate = null);

public record ImportResultDto(
    int Imported,
    int Skipped,
    IReadOnlyList<string> Errors);
