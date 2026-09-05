using Mondabet.Shared.Domain;

namespace Mondabet.Employee.Domain.Entities;

public class Department : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string? NameAr { get; private set; }
    public string? Code { get; private set; }
    public Guid? ParentId { get; private set; }
    public Guid? ManagerId { get; private set; }

    protected Department() { }

    public static Department Create(
        string name,
        string? nameAr = null,
        string? code = null,
        Guid? parentId = null,
        Guid? managerId = null)
        => new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            NameAr = nameAr,
            Code = code,
            ParentId = parentId,
            ManagerId = managerId,
        };

    public void SetManager(Guid managerId) => ManagerId = managerId;
    public void ClearManager() => ManagerId = null;
    public void Rename(string name) => Name = name;

    public void Update(string name, string? nameAr, string? code, Guid? parentId, Guid? managerId)
    {
        Name = name;
        NameAr = nameAr;
        Code = code;
        ParentId = parentId;
        ManagerId = managerId;
    }
}
