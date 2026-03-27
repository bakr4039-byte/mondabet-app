using MediatR;
using Mondabet.Notification.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Notification.Application.Commands.SendBulk;

public record SendBulkSmsCommand(BulkSmsDto Dto) : IRequest<Result<bool>>;
public record SendBulkPushCommand(BulkPushDto Dto) : IRequest<Result<bool>>;
