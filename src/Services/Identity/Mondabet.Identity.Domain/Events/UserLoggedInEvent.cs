using Mondabet.Shared.Domain;

namespace Mondabet.Identity.Domain.Events;

public record UserLoggedInEvent(Guid UserId, Guid? TenantId, DateTime LoggedInAt) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
