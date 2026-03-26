using MediatR;
using Mondabet.Shared.Domain;
using Mondabet.Tenant.Application.DTOs;

namespace Mondabet.Tenant.Application.Queries.GetTenant;

public record GetTenantQuery(Guid TenantId) : IRequest<Result<TenantDto>>;
