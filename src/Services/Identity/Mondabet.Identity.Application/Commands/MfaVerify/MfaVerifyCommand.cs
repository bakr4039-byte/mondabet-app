using MediatR;
using Mondabet.Identity.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Identity.Application.Commands.MfaVerify;

public record MfaVerifyCommand(string SessionToken, string Otp) : IRequest<Result<AuthTokensDto>>;
