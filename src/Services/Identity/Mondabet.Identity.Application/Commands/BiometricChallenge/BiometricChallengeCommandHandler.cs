using MediatR;
using Mondabet.Identity.Application.DTOs;
using Mondabet.Identity.Application.Interfaces;
using Mondabet.Shared.Domain;

namespace Mondabet.Identity.Application.Commands.BiometricChallenge;

public class BiometricChallengeCommandHandler
    : IRequestHandler<BiometricChallengeCommand, Result<BiometricChallengeResponse>>
{
    private readonly IBiometricDeviceRepository _deviceRepository;
    private readonly IBiometricService _biometricService;

    public BiometricChallengeCommandHandler(
        IBiometricDeviceRepository deviceRepository,
        IBiometricService biometricService)
    {
        _deviceRepository = deviceRepository;
        _biometricService = biometricService;
    }

    public async Task<Result<BiometricChallengeResponse>> Handle(
        BiometricChallengeCommand request,
        CancellationToken cancellationToken)
    {
        var device = await _deviceRepository.GetByDeviceIdAsync(request.DeviceId, cancellationToken);
        if (device is null || !device.IsEnabled)
            return Error.NotFound("BiometricDevice", Guid.Empty);

        var challenge = await _biometricService.GenerateChallengeAsync(request.DeviceId, cancellationToken);
        return new BiometricChallengeResponse(challenge);
    }
}
