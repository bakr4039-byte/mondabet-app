using MediatR;
using Mondabet.Shared.Domain;
using Mondabet.Tenant.Application.DTOs;

namespace Mondabet.Tenant.Application.Commands.CreatePackage;

public record CreatePackageCommand(PackageCreateDto Dto) : IRequest<Result<PackageDto>>;
