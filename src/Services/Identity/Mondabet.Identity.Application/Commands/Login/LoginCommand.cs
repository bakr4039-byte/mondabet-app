using MediatR;
using Mondabet.Identity.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Identity.Application.Commands.Login;

public record LoginCommand(string Identifier, string Password, string TenantCode)
    : IRequest<Result<LoginResponse>>;
