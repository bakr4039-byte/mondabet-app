using MediatR;
using Mondabet.Employee.Application.Commands.CreateDepartment;
using Mondabet.Employee.Application.DTOs;
using Mondabet.Employee.Application.Interfaces;
using Mondabet.Shared.Domain;

namespace Mondabet.Employee.Application.Queries.GetDepartment;

public class GetDepartmentQueryHandler : IRequestHandler<GetDepartmentQuery, Result<DepartmentDto>>
{
    private readonly IDepartmentRepository _repo;
    private readonly IEmployeeRepository _employeeRepo;

    public GetDepartmentQueryHandler(IDepartmentRepository repo, IEmployeeRepository employeeRepo)
    {
        _repo = repo;
        _employeeRepo = employeeRepo;
    }

    public async Task<Result<DepartmentDto>> Handle(GetDepartmentQuery request, CancellationToken ct)
    {
        var department = await _repo.GetByIdAsync(request.Id, ct);
        if (department is null) return Error.NotFound("Department", request.Id);

        return await CreateDepartmentCommandHandler.ToDto(department, _employeeRepo, ct);
    }
}
