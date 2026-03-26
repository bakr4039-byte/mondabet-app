using MediatR;
using Mondabet.Identity.Application.DTOs;
using Mondabet.Identity.Application.Interfaces;
using Mondabet.Shared.Domain;

namespace Mondabet.Identity.Application.Commands.NafathInitiate;

public class NafathInitiateCommandHandler
    : IRequestHandler<NafathInitiateCommand, Result<NafathTransactionResponse>>
{
    private readonly INafathService _nafathService;

    public NafathInitiateCommandHandler(INafathService nafathService)
        => _nafathService = nafathService;

    public async Task<Result<NafathTransactionResponse>> Handle(
        NafathInitiateCommand request,
        CancellationToken cancellationToken)
    {
        var transactionId = await _nafathService.InitiateAsync(request.IqamaNumber, cancellationToken);
        return new NafathTransactionResponse(transactionId);
    }
}
