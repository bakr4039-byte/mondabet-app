using MediatR;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;
using Mondabet.Tenant.Application.DTOs;
using Mondabet.Tenant.Application.Interfaces;
using TenantEntity = Mondabet.Tenant.Domain.Entities.Tenant;

namespace Mondabet.Tenant.Application.Commands.CreateTenant;

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, Result<TenantDto>>
{
    private readonly ITenantRepository _tenantRepo;
    private readonly IPackageRepository _packageRepo;
    private readonly IUnitOfWork _uow;

    public CreateTenantCommandHandler(
        ITenantRepository tenantRepo,
        IPackageRepository packageRepo,
        IUnitOfWork uow)
    {
        _tenantRepo = tenantRepo;
        _packageRepo = packageRepo;
        _uow = uow;
    }

    public async Task<Result<TenantDto>> Handle(CreateTenantCommand request, CancellationToken ct)
    {
        var dto = request.Dto;

        if (await _tenantRepo.CodeExistsAsync(dto.Code, ct))
            return Error.Conflict($"Tenant code '{dto.Code}' is already taken.");

        if (!await _packageRepo.ExistsAsync(dto.PackageId, ct))
            return Error.NotFound("Package", dto.PackageId);

        var tenant = TenantEntity.Create(
            dto.Code, dto.CompanyName, dto.AdminEmail, dto.AdminMobile,
            dto.PackageId, dto.SubscriptionEndDate,
            dto.PrimaryColor, dto.SecondaryColor);

        tenant.Update(dto.CompanyName, dto.LogoUrl, dto.PrimaryColor, dto.SecondaryColor,
            dto.Slogan, dto.Address, dto.NationalAddress,
            dto.Latitude, dto.Longitude, dto.BankAccount, dto.ZakatNumber);

        await _tenantRepo.AddAsync(tenant, ct);
        await _uow.SaveChangesAsync(ct);

        return ToDto(tenant);
    }

    internal static TenantDto ToDto(TenantEntity t) => new(
        t.Id, t.Code, t.CompanyName, t.LogoUrl, t.PrimaryColor, t.SecondaryColor,
        t.Slogan, t.Address, t.NationalAddress, t.Latitude, t.Longitude,
        t.BankAccount, t.ZakatNumber, t.SubscriptionEndDate,
        t.PackageId, t.IsActive, t.AdminEmail, t.AdminMobile);
}
