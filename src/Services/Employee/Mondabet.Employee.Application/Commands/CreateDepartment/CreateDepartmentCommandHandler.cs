using MediatR;
using Mondabet.Employee.Application.DTOs;
using Mondabet.Employee.Application.Interfaces;
using Mondabet.Employee.Domain.Entities;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;

namespace Mondabet.Employee.Application.Commands.CreateDepartment;

public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, Result<DepartmentDto>>
{
    private readonly IDepartmentRepository _repo;
    private readonly IEmployeeRepository _employeeRepo;
    private readonly IUnitOfWork _uow;

    public CreateDepartmentCommandHandler(
        IDepartmentRepository repo, IEmployeeRepository employeeRepo, IUnitOfWork uow)
    {
        _repo = repo;
        _employeeRepo = employeeRepo;
        _uow = uow;
    }

    public async Task<Result<DepartmentDto>> Handle(CreateDepartmentCommand request, CancellationToken ct)
    {
        var d = request.Dto;

        if (!string.IsNullOrWhiteSpace(d.Code) && await _repo.CodeExistsAsync(d.Code, null, ct))
            return Error.Conflict($"Department code '{d.Code}' already exists.");

        var department = Department.Create(d.Name, d.NameAr, d.Code, d.ParentId, d.ManagerId);
        await _repo.AddAsync(department, ct);
        await _uow.SaveChangesAsync(ct);

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
