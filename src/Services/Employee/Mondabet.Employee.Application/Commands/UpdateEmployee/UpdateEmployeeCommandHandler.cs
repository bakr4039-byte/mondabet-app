using MediatR;
using Mondabet.Employee.Application.Commands.CreateEmployee;
using Mondabet.Employee.Application.DTOs;
using Mondabet.Employee.Application.Interfaces;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;
using Mondabet.Shared.Infrastructure.Audit;

namespace Mondabet.Employee.Application.Commands.UpdateEmployee;

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, Result<EmployeeDto>>
{
    private readonly IEmployeeRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogger _auditLogger;
    private readonly ITenantProvider _tenantProvider;
    private readonly ICurrentUserProvider _currentUser;

    public UpdateEmployeeCommandHandler(
        IEmployeeRepository repo, IUnitOfWork uow,
        IAuditLogger auditLogger, ITenantProvider tenantProvider, ICurrentUserProvider currentUser)
    {
        _repo = repo;
        _uow = uow;
        _auditLogger = auditLogger;
        _tenantProvider = tenantProvider;
        _currentUser = currentUser;
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

        await _auditLogger.LogAsync(AuditLog.Create(
            tenantId: _tenantProvider.TenantId?.ToString() ?? string.Empty,
            userId: _currentUser.UserId?.ToString() ?? string.Empty,
            action: "Employee.Update",
            entityType: "Employee",
            entityId: employee.Id.ToString(),
            ipAddress: _currentUser.IpAddress), ct);

        return CreateEmployeeCommandHandler.ToDto(employee);
    }
}
