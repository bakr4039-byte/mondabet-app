using Mondabet.Shared.Domain;

namespace Mondabet.Identity.Domain.Events;

public record TokenRefreshedEvent(Guid UserId, string OldToken, string NewToken) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
