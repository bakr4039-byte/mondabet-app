using MediatR;

namespace Mondabet.Shared.Domain;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}
