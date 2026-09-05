using MediatR;
using Mondabet.Employee.Application.Commands.CreateDepartment;
using Mondabet.Employee.Application.DTOs;
using Mondabet.Employee.Application.Interfaces;
using Mondabet.Shared.Domain;

namespace Mondabet.Employee.Application.Queries.ListDepartments;

public class ListDepartmentsQueryHandler
    : IRequestHandler<ListDepartmentsQuery, Result<IReadOnlyList<DepartmentDto>>>
{
    private readonly IDepartmentRepository _repo;
    private readonly IEmployeeRepository _employeeRepo;

    public ListDepartmentsQueryHandler(IDepartmentRepository repo, IEmployeeRepository employeeRepo)
    {
        _repo = repo;
        _employeeRepo = employeeRepo;
    }

    public async Task<Result<IReadOnlyList<DepartmentDto>>> Handle(
        ListDepartmentsQuery request, CancellationToken ct)
    {
        var departments = await _repo.GetAllAsync(ct);

        var dtos = new List<DepartmentDto>(departments.Count);
        foreach (var d in departments)
            dtos.Add(await CreateDepartmentCommandHandler.ToDto(d, _employeeRepo, ct));

        return dtos;
    }
}
