using MediatR;
using Mondabet.Notification.Application.Commands.SendBulk;
using Mondabet.Notification.Application.Commands.SendPush;
using Mondabet.Notification.Application.Commands.SendSms;
using Mondabet.Notification.Application.DTOs;

namespace Mondabet.Notification.Api.Endpoints;

public static class NotificationEndpoints
{
    public static void MapNotificationEndpoints(this WebApplication app)
    {
        var notify = app.MapGroup("/api/v1/notifications")
            .RequireAuthorization("CompanyAdmin");

        notify.MapPost("/sms", async (SendSmsDto dto, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new SendSmsCommand(dto), ct);
            return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
        });

        notify.MapPost("/push", async (SendPushDto dto, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new SendPushCommand(dto), ct);
            return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
        });

        notify.MapPost("/sms/bulk", async (BulkSmsDto dto, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new SendBulkSmsCommand(dto), ct);
            return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
        });

        notify.MapPost("/push/bulk", async (BulkPushDto dto, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new SendBulkPushCommand(dto), ct);
            return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
        });
    }
}
