using MediatR;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;
using Mondabet.Tenant.Application.Commands.CreateTenant;
using Mondabet.Tenant.Application.DTOs;
using Mondabet.Tenant.Application.Interfaces;

namespace Mondabet.Tenant.Application.Commands.UpdateTenant;

public class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand, Result<TenantDto>>
{
    private readonly ITenantRepository _tenantRepo;
    private readonly IUnitOfWork _uow;

    public UpdateTenantCommandHandler(ITenantRepository tenantRepo, IUnitOfWork uow)
    {
        _tenantRepo = tenantRepo;
        _uow = uow;
    }

    public async Task<Result<TenantDto>> Handle(UpdateTenantCommand request, CancellationToken ct)
    {
        var tenant = await _tenantRepo.GetByIdAsync(request.TenantId, ct);
        if (tenant is null) return Error.NotFound("Tenant", request.TenantId);

        var d = request.Dto;
        tenant.Update(d.CompanyName, d.LogoUrl, d.PrimaryColor, d.SecondaryColor,
            d.Slogan, d.Address, d.NationalAddress, d.Latitude, d.Longitude,
            d.BankAccount, d.ZakatNumber);

        _tenantRepo.Update(tenant);
        await _uow.SaveChangesAsync(ct);
        return CreateTenantCommandHandler.ToDto(tenant);
    }
}
