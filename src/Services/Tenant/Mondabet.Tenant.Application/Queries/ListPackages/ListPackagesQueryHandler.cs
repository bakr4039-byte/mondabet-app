using MediatR;
using Mondabet.Shared.Domain;
using Mondabet.Tenant.Application.Commands.CreatePackage;
using Mondabet.Tenant.Application.DTOs;
using Mondabet.Tenant.Application.Interfaces;

namespace Mondabet.Tenant.Application.Queries.ListPackages;

public class ListPackagesQueryHandler
    : IRequestHandler<ListPackagesQuery, Result<IReadOnlyList<PackageDto>>>
{
    private readonly IPackageRepository _packageRepo;

    public ListPackagesQueryHandler(IPackageRepository packageRepo) => _packageRepo = packageRepo;

    public async Task<Result<IReadOnlyList<PackageDto>>> Handle(
        ListPackagesQuery request, CancellationToken ct)
    {
        var packages = await _packageRepo.GetAllAsync(ct);
        return packages.Select(CreatePackageCommandHandler.ToDto).ToList();
    }
}
