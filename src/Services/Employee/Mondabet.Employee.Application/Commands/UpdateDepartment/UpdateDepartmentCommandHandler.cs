using MediatR;
using Mondabet.Employee.Application.Commands.CreateDepartment;
using Mondabet.Employee.Application.DTOs;
using Mondabet.Employee.Application.Interfaces;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;

namespace Mondabet.Employee.Application.Commands.UpdateDepartment;

public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, Result<DepartmentDto>>
{
    private readonly IDepartmentRepository _repo;
    private readonly IEmployeeRepository _employeeRepo;
    private readonly IUnitOfWork _uow;

    public UpdateDepartmentCommandHandler(
        IDepartmentRepository repo, IEmployeeRepository employeeRepo, IUnitOfWork uow)
    {
        _repo = repo;
        _employeeRepo = employeeRepo;
        _uow = uow;
    }

    public async Task<Result<DepartmentDto>> Handle(UpdateDepartmentCommand request, CancellationToken ct)
    {
        var department = await _repo.GetByIdAsync(request.Id, ct);
        if (department is null) return Error.NotFound("Department", request.Id);

        var d = request.Dto;
        if (!string.IsNullOrWhiteSpace(d.Code) && await _repo.CodeExistsAsync(d.Code, request.Id, ct))
            return Error.Conflict($"Department code '{d.Code}' already exists.");

        department.Update(d.Name, d.NameAr, d.Code, d.ParentId, d.ManagerId);
        _repo.Update(department);
        await _uow.SaveChangesAsync(ct);

        return await CreateDepartmentCommandHandler.ToDto(department, _employeeRepo, ct);
    }
}
