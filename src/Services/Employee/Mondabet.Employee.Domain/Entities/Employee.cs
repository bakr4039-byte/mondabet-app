using Mondabet.Shared.Domain;

namespace Mondabet.Employee.Domain.Entities;

public class Employee : BaseEntity
{
    public Guid UserId { get; private set; }
    public string FullNameAr { get; private set; } = default!;
    public string FullNameEn { get; private set; } = default!;
    public string Iqama { get; private set; } = default!;
    public DateOnly DateOfBirth { get; private set; }
    public string JobTitle { get; private set; } = default!;
    public string MobileNumber { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public Guid DepartmentId { get; private set; }
    public Guid? ShiftId { get; private set; }

    // Active/inactive is distinct from soft-delete (IsDeleted): a suspended employee is kept
    // for history/reporting but shouldn't be able to check in or show up as staffed.
    public bool IsActive { get; private set; } = true;

    // HR / compliance document tracking — ideas carried over from the Google AI Studio
    // prototype's Employee type. All optional: none of this is required for a plain
    // employee record to work exactly as before.
    public string? EmployeeNumber { get; private set; }
    public int? FingerprintId { get; private set; }
    public string? PinCode { get; private set; }
    public DateOnly? IqamaExpiryDate { get; private set; }
    public DateOnly? ContractExpiryDate { get; private set; }
    public DateOnly? HealthCertExpiryDate { get; private set; }
    public DateOnly? MedicalInsuranceExpiryDate { get; private set; }
    public DateOnly? DrivingLicenseExpiryDate { get; private set; }

    // Punctuality / gamification (idea from the prototype's Honor Board feature).
    public double? PunctualityScore { get; private set; }
    public int? ConsecutiveOnTimeDays { get; private set; }
    public string? GamificationBadge { get; private set; }

    // Payroll inputs. Both optional: an employee with neither set simply can't be payrolled
    // yet (the Report service's payroll report skips them rather than guessing a number).
    public decimal? BaseSalary { get; private set; }
    public decimal? HourlyRate { get; private set; }

    protected Employee() { }

    public static Employee Create(
        Guid userId,
        string fullNameAr,
        string fullNameEn,
        string iqama,
        DateOnly dateOfBirth,
        string jobTitle,
        string mobile,
        string email,
        Guid departmentId,
        Guid? shiftId = null,
        string? employeeNumber = null,
        int? fingerprintId = null,
        string? pinCode = null,
        DateOnly? iqamaExpiryDate = null,
        DateOnly? contractExpiryDate = null,
        DateOnly? healthCertExpiryDate = null,
        DateOnly? medicalInsuranceExpiryDate = null,
        DateOnly? drivingLicenseExpiryDate = null,
        decimal? baseSalary = null,
        decimal? hourlyRate = null)
        => new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FullNameAr = fullNameAr,
            FullNameEn = fullNameEn,
            Iqama = iqama,
            DateOfBirth = dateOfBirth,
            JobTitle = jobTitle,
            MobileNumber = mobile,
            Email = email,
            DepartmentId = departmentId,
            ShiftId = shiftId,
            EmployeeNumber = employeeNumber,
            FingerprintId = fingerprintId,
            PinCode = pinCode,
            IqamaExpiryDate = iqamaExpiryDate,
            ContractExpiryDate = contractExpiryDate,
            HealthCertExpiryDate = healthCertExpiryDate,
            MedicalInsuranceExpiryDate = medicalInsuranceExpiryDate,
            DrivingLicenseExpiryDate = drivingLicenseExpiryDate,
            BaseSalary = baseSalary,
            HourlyRate = hourlyRate,
        };

    public void Update(
        string fullNameAr,
        string fullNameEn,
        string jobTitle,
        string mobile,
        string email,
        Guid departmentId,
        Guid? shiftId,
        string? employeeNumber = null,
        int? fingerprintId = null,
        string? pinCode = null,
        DateOnly? iqamaExpiryDate = null,
        DateOnly? contractExpiryDate = null,
        DateOnly? healthCertExpiryDate = null,
        DateOnly? medicalInsuranceExpiryDate = null,
        DateOnly? drivingLicenseExpiryDate = null,
        decimal? baseSalary = null,
        decimal? hourlyRate = null)
    {
        FullNameAr = fullNameAr;
        FullNameEn = fullNameEn;
        JobTitle = jobTitle;
        MobileNumber = mobile;
        Email = email;
        DepartmentId = departmentId;
        ShiftId = shiftId;
        EmployeeNumber = employeeNumber;
        FingerprintId = fingerprintId;
        PinCode = pinCode;
        IqamaExpiryDate = iqamaExpiryDate;
        ContractExpiryDate = contractExpiryDate;
        HealthCertExpiryDate = healthCertExpiryDate;
        MedicalInsuranceExpiryDate = medicalInsuranceExpiryDate;
        DrivingLicenseExpiryDate = drivingLicenseExpiryDate;
        BaseSalary = baseSalary;
        HourlyRate = hourlyRate;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    /// <summary>
    /// Called by the Attendance service's daily aggregation (or a scheduled job) to keep the
    /// punctuality/gamification numbers current. Kept as plain setters here so any caller with
    /// access to the entity can recompute them — the actual scoring rule lives wherever the
    /// attendance stats are aggregated, not in this entity.
    /// </summary>
    public void UpdatePunctuality(double? score, int? consecutiveOnTimeDays, string? badge)
    {
        PunctualityScore = score;
        ConsecutiveOnTimeDays = consecutiveOnTimeDays;
        GamificationBadge = badge;
    }
}
