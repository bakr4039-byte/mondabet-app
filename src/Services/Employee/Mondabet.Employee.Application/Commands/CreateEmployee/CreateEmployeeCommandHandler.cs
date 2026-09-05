using MediatR;
using Mondabet.Employee.Application.DTOs;
using Mondabet.Employee.Application.Interfaces;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;
using EmployeeEntity = Mondabet.Employee.Domain.Entities.Employee;

namespace Mondabet.Employee.Application.Commands.CreateEmployee;

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, Result<EmployeeDto>>
{
    private readonly IEmployeeRepository _repo;
    private readonly IUnitOfWork _uow;

    public CreateEmployeeCommandHandler(IEmployeeRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<EmployeeDto>> Handle(CreateEmployeeCommand request, CancellationToken ct)
    {
        if (await _repo.IqamaExistsAsync(request.Dto.Iqama, ct))
            return Error.Conflict($"Iqama '{request.Dto.Iqama}' already exists in this tenant.");

        var d = request.Dto;
        var employee = EmployeeEntity.Create(
            Guid.NewGuid(), // UserId – will be provisioned in identity-svc in production
            d.FullNameAr, d.FullNameEn, d.Iqama, d.DateOfBirth,
            d.JobTitle, d.MobileNumber, d.Email, d.DepartmentId, d.ShiftId,
            d.EmployeeNumber, d.FingerprintId, d.PinCode,
            d.IqamaExpiryDate, d.ContractExpiryDate, d.HealthCertExpiryDate,
            d.MedicalInsuranceExpiryDate, d.DrivingLicenseExpiryDate);

        await _repo.AddAsync(employee, ct);
        await _uow.SaveChangesAsync(ct);
        return ToDto(employee);
    }

    internal static EmployeeDto ToDto(EmployeeEntity e) => new(
        e.Id, e.UserId, e.FullNameAr, e.FullNameEn, e.Iqama,
        e.DateOfBirth, e.JobTitle, e.MobileNumber, e.Email,
        e.DepartmentId, e.ShiftId, e.IsActive,
        e.EmployeeNumber, e.FingerprintId, e.PinCode,
        e.IqamaExpiryDate, e.ContractExpiryDate, e.HealthCertExpiryDate,
        e.MedicalInsuranceExpiryDate, e.DrivingLicenseExpiryDate,
        e.PunctualityScore, e.ConsecutiveOnTimeDays, e.GamificationBadge);
}
