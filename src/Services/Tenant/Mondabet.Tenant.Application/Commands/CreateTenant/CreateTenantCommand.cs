using MediatR;
using Mondabet.Shared.Domain;
using Mondabet.Tenant.Application.DTOs;

namespace Mondabet.Tenant.Application.Commands.CreateTenant;

public record CreateTenantCommand(TenantCreateDto Dto) : IRequest<Result<TenantDto>>;
