using MediatR;
using Mondabet.Notification.Application.Interfaces;
using Mondabet.Shared.Domain;

namespace Mondabet.Notification.Application.Commands.SendPush;

public class SendPushCommandHandler : IRequestHandler<SendPushCommand, Result<bool>>
{
    private readonly IPushService _push;
    public SendPushCommandHandler(IPushService push) => _push = push;

    public async Task<Result<bool>> Handle(SendPushCommand request, CancellationToken ct)
    {
        await _push.SendAsync(
            request.Dto.DeviceToken, request.Dto.Title,
            request.Dto.Body, request.Dto.Data, ct);
        return true;
    }
}
