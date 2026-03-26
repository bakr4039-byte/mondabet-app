using MediatR;
using Mondabet.Employee.Application.Interfaces;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;

namespace Mondabet.Employee.Application.Commands.DeleteEmployee;

public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, Result<bool>>
{
    private readonly IEmployeeRepository _repo;
    private readonly IUnitOfWork _uow;

    public DeleteEmployeeCommandHandler(IEmployeeRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result<bool>> Handle(DeleteEmployeeCommand request, CancellationToken ct)
    {
        var employee = await _repo.GetByIdAsync(request.EmployeeId, ct);
        if (employee is null) return Error.NotFound("Employee", request.EmployeeId);

        _repo.Delete(employee);
        await _uow.SaveChangesAsync(ct);
        return true;
    }
}
