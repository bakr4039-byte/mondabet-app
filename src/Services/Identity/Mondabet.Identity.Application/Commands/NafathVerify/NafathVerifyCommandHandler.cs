using MediatR;
using Mondabet.Identity.Application.DTOs;
using Mondabet.Identity.Application.Interfaces;
using Mondabet.Identity.Domain.Entities;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;

namespace Mondabet.Identity.Application.Commands.NafathVerify;

public class NafathVerifyCommandHandler : IRequestHandler<NafathVerifyCommand, Result<AuthTokensDto>>
{
    private readonly INafathService _nafathService;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IKeycloakService _keycloakService;
    private readonly IUnitOfWork _unitOfWork;

    public NafathVerifyCommandHandler(
        INafathService nafathService,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtTokenService jwtTokenService,
        IKeycloakService keycloakService,
        IUnitOfWork unitOfWork)
    {
        _nafathService = nafathService;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenService = jwtTokenService;
        _keycloakService = keycloakService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthTokensDto>> Handle(
        NafathVerifyCommand request,
        CancellationToken cancellationToken)
    {
        var isVerified = await _nafathService.VerifyAsync(request.TransactionId, cancellationToken);
        if (!isVerified)
            return Error.Unauthorized("Nafath verification pending or failed.");

        var user = await _userRepository.GetByMobileAsync(request.IqamaNumber, cancellationToken);
        if (user is null || !user.IsActive)
            return Error.Unauthorized("User not found or inactive.");

        var roles = await _keycloakService.GetUserRolesAsync(user.Id.ToString(), cancellationToken);
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
        var rawRefreshToken = _jwtTokenService.GenerateRefreshToken();

        var refreshToken = RefreshToken.Create(user.Id, rawRefreshToken, expiryDays: 30);
        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthTokensDto(accessToken, rawRefreshToken);
    }
}
