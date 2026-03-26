using MediatR;
using Mondabet.Shared.Domain;
using Mondabet.Tenant.Application.Commands.CreateTenant;
using Mondabet.Tenant.Application.DTOs;
using Mondabet.Tenant.Application.Interfaces;

namespace Mondabet.Tenant.Application.Queries.GetTenant;

public class GetTenantQueryHandler : IRequestHandler<GetTenantQuery, Result<TenantDto>>
{
    private readonly ITenantRepository _tenantRepo;

    public GetTenantQueryHandler(ITenantRepository tenantRepo) => _tenantRepo = tenantRepo;

    public async Task<Result<TenantDto>> Handle(GetTenantQuery request, CancellationToken ct)
    {
        var tenant = await _tenantRepo.GetByIdAsync(request.TenantId, ct);
        if (tenant is null) return Error.NotFound("Tenant", request.TenantId);
        return CreateTenantCommandHandler.ToDto(tenant);
    }
}
