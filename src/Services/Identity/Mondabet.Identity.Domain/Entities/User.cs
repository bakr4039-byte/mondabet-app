using Mondabet.Identity.Domain.Events;
using Mondabet.Shared.Domain;

namespace Mondabet.Identity.Domain.Entities;

public class User : BaseEntity
{
    public Guid? TenantId { get; private set; }
    public string Email { get; private set; } = default!;
    public string MobileNumber { get; private set; } = default!;
    public Guid? RoleId { get; private set; }
    public bool IsActive { get; private set; }

    protected User() { }

    public static User Create(Guid? tenantId, string email, string mobileNumber)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = email.ToLowerInvariant(),
            MobileNumber = mobileNumber,
            IsActive = true
        };
        user.AddDomainEvent(new UserCreatedEvent(user.Id, email, tenantId));
        return user;
    }

    public void AssignRole(Guid roleId) => RoleId = roleId;
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
