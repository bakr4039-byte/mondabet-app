using MediatR;
using Mondabet.Identity.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Identity.Application.Commands.NafathVerify;

public record NafathVerifyCommand(string TransactionId, string IqamaNumber)
    : IRequest<Result<AuthTokensDto>>;
