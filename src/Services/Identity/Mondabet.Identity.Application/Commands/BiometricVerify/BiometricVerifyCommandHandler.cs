using MediatR;
using Mondabet.Identity.Application.DTOs;
using Mondabet.Identity.Application.Interfaces;
using Mondabet.Identity.Domain.Entities;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;

namespace Mondabet.Identity.Application.Commands.BiometricVerify;

public class BiometricVerifyCommandHandler : IRequestHandler<BiometricVerifyCommand, Result<AuthTokensDto>>
{
    private readonly IBiometricDeviceRepository _deviceRepository;
    private readonly IBiometricService _biometricService;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IKeycloakService _keycloakService;
    private readonly IUnitOfWork _unitOfWork;

    public BiometricVerifyCommandHandler(
        IBiometricDeviceRepository deviceRepository,
        IBiometricService biometricService,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtTokenService jwtTokenService,
        IKeycloakService keycloakService,
        IUnitOfWork unitOfWork)
    {
        _deviceRepository = deviceRepository;
        _biometricService = biometricService;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenService = jwtTokenService;
        _keycloakService = keycloakService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthTokensDto>> Handle(
        BiometricVerifyCommand request,
        CancellationToken cancellationToken)
    {
        var device = await _deviceRepository.GetByDeviceIdAsync(request.DeviceId, cancellationToken);
        if (device is null || !device.IsEnabled)
            return Error.Unauthorized("Device not registered or disabled.");

        var isValid = await _biometricService.VerifySignatureAsync(
            request.DeviceId, string.Empty, request.SignedChallenge, device.PublicKey, cancellationToken);

        if (!isValid)
            return Error.Unauthorized("Biometric verification failed.");

        var user = await _userRepository.GetByIdAsync(device.UserId, cancellationToken);
        if (user is null || !user.IsActive)
            return Error.Unauthorized("User not found or inactive.");

        var roles = await _keycloakService.GetUserRolesAsync(user.Id.ToString(), cancellationToken);
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
        var rawRefreshToken = _jwtTokenService.GenerateRefreshToken();

        var refreshToken = RefreshToken.Create(user.Id, rawRefreshToken, expiryDays: 30, request.DeviceId);
        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthTokensDto(accessToken, rawRefreshToken);
    }
}
