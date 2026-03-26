using MediatR;
using Mondabet.Identity.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Identity.Application.Commands.NafathInitiate;

public record NafathInitiateCommand(string IqamaNumber) : IRequest<Result<NafathTransactionResponse>>;
