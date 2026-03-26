using MediatR;
using Mondabet.Identity.Application.DTOs;
using Mondabet.Identity.Application.Interfaces;
using Mondabet.Identity.Domain.Entities;
using Mondabet.Identity.Domain.Events;
using Mondabet.Shared.Application;
using Mondabet.Shared.Domain;

namespace Mondabet.Identity.Application.Commands.MfaVerify;

public class MfaVerifyCommandHandler : IRequestHandler<MfaVerifyCommand, Result<AuthTokensDto>>
{
    private readonly IOtpService _otpService;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IKeycloakService _keycloakService;
    private readonly IUnitOfWork _unitOfWork;

    public MfaVerifyCommandHandler(
        IOtpService otpService,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtTokenService jwtTokenService,
        IKeycloakService keycloakService,
        IUnitOfWork unitOfWork)
    {
        _otpService = otpService;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenService = jwtTokenService;
        _keycloakService = keycloakService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthTokensDto>> Handle(MfaVerifyCommand request, CancellationToken cancellationToken)
    {
        var isValidOtp = await _otpService.VerifyOtpAsync(request.SessionToken, request.Otp, cancellationToken);
        if (!isValidOtp)
            return Error.Unauthorized("Invalid or expired OTP.");

        var userId = await _otpService.GetUserIdFromSessionAsync(request.SessionToken, cancellationToken);
        if (userId is null)
            return Error.Unauthorized("Session expired.");

        var user = await _userRepository.GetByIdAsync(userId.Value, cancellationToken);
        if (user is null || !user.IsActive)
            return Error.Unauthorized("User not found or inactive.");

        var roles = await _keycloakService.GetUserRolesAsync(user.Id.ToString(), cancellationToken);
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
        var rawRefreshToken = _jwtTokenService.GenerateRefreshToken();

        var refreshToken = RefreshToken.Create(user.Id, rawRefreshToken, expiryDays: 30);
        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        user.AddDomainEvent(new UserLoggedInEvent(user.Id, user.TenantId, DateTime.UtcNow));
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthTokensDto(accessToken, rawRefreshToken);
    }
}
