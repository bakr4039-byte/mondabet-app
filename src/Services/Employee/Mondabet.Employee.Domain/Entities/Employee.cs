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
        Guid? shiftId = null)
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
            ShiftId = shiftId
        };

    public void Update(
        string fullNameAr,
        string fullNameEn,
        string jobTitle,
        string mobile,
        string email,
        Guid departmentId,
        Guid? shiftId)
    {
        FullNameAr = fullNameAr;
        FullNameEn = fullNameEn;
        JobTitle = jobTitle;
        MobileNumber = mobile;
        Email = email;
        DepartmentId = departmentId;
        ShiftId = shiftId;
    }
}
