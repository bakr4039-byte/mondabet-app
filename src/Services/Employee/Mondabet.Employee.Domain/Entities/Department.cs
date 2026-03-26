using Mondabet.Shared.Domain;

namespace Mondabet.Employee.Domain.Entities;

public class Department : BaseEntity
{
    public string Name { get; private set; } = default!;
    public Guid? ParentId { get; private set; }
    public Guid? ManagerId { get; private set; }

    protected Department() { }

    public static Department Create(string name, Guid? parentId = null)
        => new() { Id = Guid.NewGuid(), Name = name, ParentId = parentId };

    public void SetManager(Guid managerId) => ManagerId = managerId;
    public void Rename(string name) => Name = name;
}
