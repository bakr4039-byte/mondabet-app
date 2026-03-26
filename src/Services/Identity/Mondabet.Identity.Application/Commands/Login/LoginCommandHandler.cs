using MediatR;
using Mondabet.Identity.Application.DTOs;
using Mondabet.Identity.Application.Interfaces;
using Mondabet.Shared.Domain;

namespace Mondabet.Identity.Application.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IKeycloakService _keycloakService;
    private readonly IOtpService _otpService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IKeycloakService keycloakService,
        IOtpService otpService)
    {
        _userRepository = userRepository;
        _keycloakService = keycloakService;
        _otpService = otpService;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var isValid = await _keycloakService.ValidateCredentialsAsync(
            request.Identifier, request.Password, cancellationToken);

        if (!isValid)
            return Error.Unauthorized("Invalid credentials.");

        var user = await _userRepository.GetByEmailAsync(request.Identifier, cancellationToken)
            ?? await _userRepository.GetByMobileAsync(request.Identifier, cancellationToken);

        if (user is null)
            return Error.Unauthorized("User not found.");

        if (!user.IsActive)
            return Error.Forbidden("Account is deactivated.");

        var sessionToken = Guid.NewGuid().ToString("N");
        await _otpService.StoreSessionAsync(sessionToken, user.Id, cancellationToken);
        await _otpService.GenerateAndSendOtpAsync(MaskMobile(user.MobileNumber), cancellationToken);

        return new LoginResponse(null, null, MfaRequired: true, SessionToken: sessionToken);
    }

    private static string MaskMobile(string mobile)
        => mobile.Length > 7
            ? mobile[..4] + new string('*', mobile.Length - 7) + mobile[^3..]
            : mobile;
}
