using MediatR;
using Mondabet.Employee.Application.DTOs;
using Mondabet.Employee.Application.Interfaces;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;
using EmployeeEntity = Mondabet.Employee.Domain.Entities.Employee;

namespace Mondabet.Employee.Application.Commands.ImportEmployees;

public class ImportEmployeesCommandHandler
    : IRequestHandler<ImportEmployeesCommand, Result<ImportResultDto>>
{
    private readonly IEmployeeRepository _repo;
    private readonly IExcelImportService _excelService;
    private readonly IUnitOfWork _uow;

    public ImportEmployeesCommandHandler(
        IEmployeeRepository repo,
        IExcelImportService excelService,
        IUnitOfWork uow)
    {
        _repo = repo;
        _excelService = excelService;
        _uow = uow;
    }

    public async Task<Result<ImportResultDto>> Handle(
        ImportEmployeesCommand request, CancellationToken ct)
    {
        var rows = await _excelService.ParseEmployeeExcelAsync(request.ExcelStream, ct);

        int imported = 0, skipped = 0;
        var errors = new List<string>();

        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.Iqama))
            {
                errors.Add($"Row {row.RowNumber}: Iqama is required.");
                skipped++;
                continue;
            }

            if (await _repo.IqamaExistsAsync(row.Iqama, ct))
            {
                skipped++;
                continue;
            }

            if (!DateOnly.TryParse(row.DateOfBirth, out var dob))
            {
                errors.Add($"Row {row.RowNumber}: Invalid DateOfBirth '{row.DateOfBirth}'.");
                skipped++;
                continue;
            }

            var employee = EmployeeEntity.Create(
                Guid.NewGuid(), row.FullNameAr, row.FullNameEn, row.Iqama, dob,
                row.JobTitle, row.MobileNumber, row.Email,
                row.DepartmentId, row.ShiftId);

            await _repo.AddAsync(employee, ct);
            imported++;
        }

        await _uow.SaveChangesAsync(ct);
        return new ImportResultDto(imported, skipped, errors);
    }
}
