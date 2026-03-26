using MediatR;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;
using Mondabet.Tenant.Application.Commands.CreateTenant;
using Mondabet.Tenant.Application.DTOs;
using Mondabet.Tenant.Application.Interfaces;

namespace Mondabet.Tenant.Application.Commands.UpdateSubscription;

public class UpdateSubscriptionCommandHandler
    : IRequestHandler<UpdateSubscriptionCommand, Result<TenantDto>>
{
    private readonly ITenantRepository _tenantRepo;
    private readonly IPackageRepository _packageRepo;
    private readonly IUnitOfWork _uow;

    public UpdateSubscriptionCommandHandler(
        ITenantRepository tenantRepo, IPackageRepository packageRepo, IUnitOfWork uow)
    {
        _tenantRepo = tenantRepo;
        _packageRepo = packageRepo;
        _uow = uow;
    }

    public async Task<Result<TenantDto>> Handle(UpdateSubscriptionCommand request, CancellationToken ct)
    {
        var tenant = await _tenantRepo.GetByIdAsync(request.TenantId, ct);
        if (tenant is null) return Error.NotFound("Tenant", request.TenantId);

        if (!await _packageRepo.ExistsAsync(request.Dto.PackageId, ct))
            return Error.NotFound("Package", request.Dto.PackageId);

        tenant.UpdateSubscription(request.Dto.PackageId, request.Dto.EndDate);
        _tenantRepo.Update(tenant);
        await _uow.SaveChangesAsync(ct);
        return CreateTenantCommandHandler.ToDto(tenant);
    }
}
