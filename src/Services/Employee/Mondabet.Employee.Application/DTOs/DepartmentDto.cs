namespace Mondabet.Employee.Application.DTOs;

public record DepartmentDto(
    Guid Id,
    string Name,
    string? NameAr,
    string? Code,
    Guid? ParentId,
    Guid? ManagerId,
    string? ManagerName,
    int EmployeeCount);

public record DepartmentCreateDto(
    string Name,
    string? NameAr,
    string? Code,
    Guid? ParentId = null,
    Guid? ManagerId = null);

public record DepartmentUpdateDto(
    string Name,
    string? NameAr,
    string? Code,
    Guid? ParentId = null,
    Guid? ManagerId = null);
