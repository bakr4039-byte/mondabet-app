using MediatR;
using Mondabet.Shared.Domain;
using Mondabet.Tenant.Application.DTOs;

namespace Mondabet.Tenant.Application.Queries.ListPackages;

public record ListPackagesQuery : IRequest<Result<IReadOnlyList<PackageDto>>>;
