using Mondabet.Shared.Domain;

namespace Mondabet.Tenant.Domain.Events;

public record TenantCreatedEvent(Guid TenantId, string Code, string AdminEmail) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
