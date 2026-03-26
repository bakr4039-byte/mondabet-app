using MediatR;
using Mondabet.Employee.Application.Commands.CreateEmployee;
using Mondabet.Employee.Application.DTOs;
using Mondabet.Employee.Application.Interfaces;
using Mondabet.Shared.Domain;

namespace Mondabet.Employee.Application.Queries.GetEmployee;

public class GetEmployeeQueryHandler : IRequestHandler<GetEmployeeQuery, Result<EmployeeDto>>
{
    private readonly IEmployeeRepository _repo;
    public GetEmployeeQueryHandler(IEmployeeRepository repo) => _repo = repo;

    public async Task<Result<EmployeeDto>> Handle(GetEmployeeQuery request, CancellationToken ct)
    {
        var employee = await _repo.GetByIdAsync(request.EmployeeId, ct);
        if (employee is null) return Error.NotFound("Employee", request.EmployeeId);
        return CreateEmployeeCommandHandler.ToDto(employee);
    }
}
