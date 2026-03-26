using MediatR;
using Mondabet.Shared.Domain;
using Mondabet.Tenant.Application.DTOs;

namespace Mondabet.Tenant.Application.Commands.UpdateTenant;

public record UpdateTenantCommand(Guid TenantId, TenantUpdateDto Dto) : IRequest<Result<TenantDto>>;
