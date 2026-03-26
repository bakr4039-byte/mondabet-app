using Mondabet.Shared.Domain;

namespace Mondabet.Identity.Domain.Events;

public record UserCreatedEvent(Guid UserId, string Email, Guid? TenantId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
