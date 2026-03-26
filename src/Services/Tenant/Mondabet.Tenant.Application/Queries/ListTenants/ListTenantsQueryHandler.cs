using MediatR;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;
using Mondabet.Tenant.Application.Commands.CreateTenant;
using Mondabet.Tenant.Application.DTOs;
using Mondabet.Tenant.Application.Interfaces;

namespace Mondabet.Tenant.Application.Queries.ListTenants;

public class ListTenantsQueryHandler
    : IRequestHandler<ListTenantsQuery, Result<PagedResult<TenantDto>>>
{
    private readonly ITenantRepository _tenantRepo;

    public ListTenantsQueryHandler(ITenantRepository tenantRepo) => _tenantRepo = tenantRepo;

    public async Task<Result<PagedResult<TenantDto>>> Handle(
        ListTenantsQuery request, CancellationToken ct)
    {
        var all = await _tenantRepo.GetAllAsync(ct);

        var filtered = string.IsNullOrWhiteSpace(request.Search)
            ? all
            : all.Where(t =>
                t.CompanyName.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                t.Code.Contains(request.Search, StringComparison.OrdinalIgnoreCase))
              .ToList();

        var totalCount = filtered.Count();
        var items = filtered
            .Skip((request.Page - 1) * request.Size)
            .Take(request.Size)
            .Select(CreateTenantCommandHandler.ToDto)
            .ToList();

        return new PagedResult<TenantDto>(items, totalCount, request.Page, request.Size);
    }
}
