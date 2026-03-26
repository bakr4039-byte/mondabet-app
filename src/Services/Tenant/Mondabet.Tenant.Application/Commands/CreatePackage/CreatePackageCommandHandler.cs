using MediatR;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;
using Mondabet.Tenant.Application.DTOs;
using Mondabet.Tenant.Application.Interfaces;
using Mondabet.Tenant.Domain.Entities;

namespace Mondabet.Tenant.Application.Commands.CreatePackage;

public class CreatePackageCommandHandler : IRequestHandler<CreatePackageCommand, Result<PackageDto>>
{
    private readonly IPackageRepository _packageRepo;
    private readonly IUnitOfWork _uow;

    public CreatePackageCommandHandler(IPackageRepository packageRepo, IUnitOfWork uow)
    {
        _packageRepo = packageRepo;
        _uow = uow;
    }

    public async Task<Result<PackageDto>> Handle(CreatePackageCommand request, CancellationToken ct)
    {
        var d = request.Dto;
        var package = Package.Create(d.Name, d.MaxUsers, d.PriceMonthly, d.FeaturesJson);
        await _packageRepo.AddAsync(package, ct);
        await _uow.SaveChangesAsync(ct);
        return ToDto(package);
    }

    internal static PackageDto ToDto(Package p) =>
        new(p.Id, p.Name, p.MaxUsers, p.PriceMonthly, p.FeaturesJson);
}
