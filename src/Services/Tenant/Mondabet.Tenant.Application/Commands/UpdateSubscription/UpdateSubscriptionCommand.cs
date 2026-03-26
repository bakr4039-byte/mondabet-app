using MediatR;
using Mondabet.Shared.Domain;
using Mondabet.Tenant.Application.DTOs;

namespace Mondabet.Tenant.Application.Commands.UpdateSubscription;

public record UpdateSubscriptionCommand(Guid TenantId, SubscriptionUpdateDto Dto)
    : IRequest<Result<TenantDto>>;
