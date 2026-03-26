using MediatR;
using Mondabet.Shared.Domain;

namespace Mondabet.Identity.Application.Commands.Logout;

public record LogoutCommand(Guid UserId, string? RefreshToken) : IRequest<Result<bool>>;
