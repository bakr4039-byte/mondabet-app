using MediatR;
using Mondabet.Employee.Application.DTOs;
using Mondabet.Employee.Application.Interfaces;
using Mondabet.Employee.Domain.Entities;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;
using Mondabet.Shared.Infrastructure.Audit;

namespace Mondabet.Employee.Application.Commands.CreateDepartment;

public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, Result<DepartmentDto>>
{
    private readonly IDepartmentRepository _repo;
    private readonly IEmployeeRepository _employeeRepo;
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogger _auditLogger;
    private readonly ITenantProvider _tenantProvider;
    private readonly ICurrentUserProvider _currentUser;

    public CreateDepartmentCommandHandler(
        IDepartmentRepository repo, IEmployeeRepository employeeRepo, IUnitOfWork uow,
        IAuditLogger auditLogger, ITenantProvider tenantProvider, ICurrentUserProvider currentUser)
    {
        _repo = repo;
        _employeeRepo = employeeRepo;
        _uow = uow;
        _auditLogger = auditLogger;
        _tenantProvider = tenantProvider;
        _currentUser = currentUser;
    }

    public async Task<Result<DepartmentDto>> Handle(CreateDepartmentCommand request, CancellationToken ct)
    {
        var d = request.Dto;

        if (!string.IsNullOrWhiteSpace(d.Code) && await _repo.CodeExistsAsync(d.Code, null, ct))
            return Error.Conflict($"Department code '{d.Code}' already exists.");

        var department = Department.Create(d.Name, d.NameAr, d.Code, d.ParentId, d.ManagerId);
        await _repo.AddAsync(department, ct);
        await _uow.SaveChangesAsync(ct);

        await _auditLogger.LogAsync(AuditLog.Create(
            tenantId: _tenantProvider.TenantId?.ToString() ?? string.Empty,
            userId: _currentUser.UserId?.ToString() ?? string.Empty,
            action: "Department.Create",
            entityType: "Department",
            entityId: department.Id.ToString(),
            ipAddress: _currentUser.IpAddress), ct);

        return await ToDto(department, _employeeRepo, ct);
    }

    internal static async Task<DepartmentDto> ToDto(
        Department d, IEmployeeRepository employeeRepo, CancellationToken ct)
    {
        string? managerName = null;
        if (d.ManagerId.HasValue)
        {
            var manager = await employeeRepo.GetByIdAsync(d.ManagerId.Value, ct);
            managerName = manager?.FullNameEn;
        }

        var count = await employeeRepo.CountByDepartmentAsync(d.Id, ct);

        return new DepartmentDto(
            d.Id, d.Name, d.NameAr, d.Code, d.ParentId, d.ManagerId, managerName, count);
    }
}
