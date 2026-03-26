using MediatR;
using Mondabet.Identity.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Identity.Application.Commands.Refresh;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthTokensDto>>;
