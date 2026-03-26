using MediatR;
using Mondabet.Employee.Application.Commands.CreateEmployee;
using Mondabet.Employee.Application.DTOs;
using Mondabet.Employee.Application.Interfaces;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;

namespace Mondabet.Employee.Application.Queries.ListEmployees;

public class ListEmployeesQueryHandler
    : IRequestHandler<ListEmployeesQuery, Result<PagedResult<EmployeeDto>>>
{
    private readonly IEmployeeRepository _repo;
    public ListEmployeesQueryHandler(IEmployeeRepository repo) => _repo = repo;

    public async Task<Result<PagedResult<EmployeeDto>>> Handle(
        ListEmployeesQuery request, CancellationToken ct)
    {
        var (items, total) = await _repo.GetPagedAsync(request.Page, request.Size, request.Search, ct);
        var dtos = items.Select(CreateEmployeeCommandHandler.ToDto).ToList();
        return new PagedResult<EmployeeDto>(dtos, total, request.Page, request.Size);
    }
}
