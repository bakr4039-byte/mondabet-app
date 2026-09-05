using MediatR;
using Mondabet.Employee.Application.Commands.CreateEmployee;
using Mondabet.Employee.Application.DTOs;
using Mondabet.Employee.Application.Interfaces;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;

namespace Mondabet.Employee.Application.Commands.UpdateEmployee;

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, Result<EmployeeDto>>
{
    private readonly IEmployeeRepository _repo;
    private readonly IUnitOfWork _uow;

    public UpdateEmployeeCommandHandler(IEmployeeRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<EmployeeDto>> Handle(UpdateEmployeeCommand request, CancellationToken ct)
    {
        var employee = await _repo.GetByIdAsync(request.EmployeeId, ct);
        if (employee is null) return Error.NotFound("Employee", request.EmployeeId);

        var d = request.Dto;
        employee.Update(d.FullNameAr, d.FullNameEn, d.JobTitle, d.MobileNumber,
            d.Email, d.DepartmentId, d.ShiftId,
            d.EmployeeNumber, d.FingerprintId, d.PinCode,
            d.IqamaExpiryDate, d.ContractExpiryDate, d.HealthCertExpiryDate,
            d.MedicalInsuranceExpiryDate, d.DrivingLicenseExpiryDate,
            d.BaseSalary, d.HourlyRate);

        _repo.Update(employee);
        await _uow.SaveChangesAsync(ct);
        return CreateEmployeeCommandHandler.ToDto(employee);
    }
}
