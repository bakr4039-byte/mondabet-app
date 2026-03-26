using MediatR;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;
using Mondabet.Tenant.Application.DTOs;

namespace Mondabet.Tenant.Application.Queries.ListTenants;

public record ListTenantsQuery(int Page = 1, int Size = 20, string? Search = null)
    : IRequest<Result<PagedResult<TenantDto>>>;
