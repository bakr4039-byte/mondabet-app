using MediatR;
using Mondabet.Identity.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Identity.Application.Commands.BiometricVerify;

public record BiometricVerifyCommand(string DeviceId, string SignedChallenge)
    : IRequest<Result<AuthTokensDto>>;
