using MediatR;
using Mondabet.Employee.Application.Interfaces;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;
using Mondabet.Shared.Infrastructure.Audit;

namespace Mondabet.Employee.Application.Commands.DeleteDepartment;

public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, Result<bool>>
{
    private readonly IDepartmentRepository _repo;
    private readonly IEmployeeRepository _employeeRepo;
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogger _auditLogger;
    private readonly ITenantProvider _tenantProvider;
    private readonly ICurrentUserProvider _currentUser;

    public DeleteDepartmentCommandHandler(
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

    public async Task<Result<bool>> Handle(DeleteDepartmentCommand request, CancellationToken ct)
    {
        var department = await _repo.GetByIdAsync(request.Id, ct);
        if (department is null) return Error.NotFound("Department", request.Id);

        // Never silently orphan employees: block deletion while anyone is still assigned.
        var count = await _employeeRepo.CountByDepartmentAsync(request.Id, ct);
        if (count > 0)
            return Error.Conflict($"Cannot delete: {count} employee(s) are still assigned to this department.");

        _repo.Delete(department);
        await _uow.SaveChangesAsync(ct);

        await _auditLogger.LogAsync(AuditLog.Create(
            tenantId: _tenantProvider.TenantId?.ToString() ?? string.Empty,
            userId: _currentUser.UserId?.ToString() ?? string.Empty,
            action: "Department.Delete",
            entityType: "Department",
            entityId: department.Id.ToString(),
            ipAddress: _currentUser.IpAddress), ct);

        return true;
    }
}
