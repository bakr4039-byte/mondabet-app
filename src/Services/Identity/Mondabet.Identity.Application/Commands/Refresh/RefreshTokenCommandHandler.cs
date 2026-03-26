using MediatR;
using Mondabet.Identity.Application.DTOs;
using Mondabet.Identity.Application.Interfaces;
using Mondabet.Identity.Domain.Entities;
using Mondabet.Identity.Domain.Events;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;

namespace Mondabet.Identity.Application.Commands.Refresh;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthTokensDto>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IKeycloakService _keycloakService;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IKeycloakService keycloakService,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _keycloakService = keycloakService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthTokensDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);

        if (storedToken is null || !storedToken.IsActive)
            return Error.Unauthorized("Refresh token is invalid or expired.");

        var user = await _userRepository.GetByIdAsync(storedToken.UserId, cancellationToken);
        if (user is null || !user.IsActive)
            return Error.Unauthorized("User not found or inactive.");

        var newRawToken = _jwtTokenService.GenerateRefreshToken();
        var newRefreshToken = RefreshToken.Create(user.Id, newRawToken, expiryDays: 30, storedToken.DeviceId);

        storedToken.Revoke(newRawToken);
        _refreshTokenRepository.Update(storedToken);
        await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

        var roles = await _keycloakService.GetUserRolesAsync(user.Id.ToString(), cancellationToken);
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);

        user.AddDomainEvent(new TokenRefreshedEvent(user.Id, request.RefreshToken, newRawToken));
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthTokensDto(accessToken, newRawToken);
    }
}
