using MediatR;
using Mondabet.Notification.Application.Interfaces;
using Mondabet.Shared.Domain;

namespace Mondabet.Notification.Application.Commands.SendSms;

public class SendSmsCommandHandler : IRequestHandler<SendSmsCommand, Result<bool>>
{
    private readonly ISmsService _sms;
    public SendSmsCommandHandler(ISmsService sms) => _sms = sms;

    public async Task<Result<bool>> Handle(SendSmsCommand request, CancellationToken ct)
    {
        await _sms.SendAsync(request.Dto.To, request.Dto.Message, ct);
        return true;
    }
}
