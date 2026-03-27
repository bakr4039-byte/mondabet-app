using MediatR;
using Mondabet.Notification.Application.DTOs;
using Mondabet.Shared.Domain;

namespace Mondabet.Notification.Application.Commands.SendSms;

public record SendSmsCommand(SendSmsDto Dto) : IRequest<Result<bool>>;
