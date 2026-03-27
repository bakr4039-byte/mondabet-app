using MediatR;
using Mondabet.Notification.Application.Interfaces;
using Mondabet.Shared.Domain;

namespace Mondabet.Notification.Application.Commands.SendBulk;

public class SendBulkSmsCommandHandler : IRequestHandler<SendBulkSmsCommand, Result<bool>>
{
    private readonly ISmsService _sms;
    public SendBulkSmsCommandHandler(ISmsService sms) => _sms = sms;

    public async Task<Result<bool>> Handle(SendBulkSmsCommand request, CancellationToken ct)
    {
        await _sms.SendBulkAsync(request.Dto.Recipients, request.Dto.Message, ct);
        return true;
    }
}

public class SendBulkPushCommandHandler : IRequestHandler<SendBulkPushCommand, Result<bool>>
{
    private readonly IPushService _push;
    public SendBulkPushCommandHandler(IPushService push) => _push = push;

    public async Task<Result<bool>> Handle(SendBulkPushCommand request, CancellationToken ct)
    {
        await _push.SendBulkAsync(
            request.Dto.DeviceTokens, request.Dto.Title,
            request.Dto.Body, request.Dto.Data, ct);
        return true;
    }
}
