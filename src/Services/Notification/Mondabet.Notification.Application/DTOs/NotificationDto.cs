namespace Mondabet.Notification.Application.DTOs;

public record SendSmsDto(string To, string Message);

public record SendPushDto(string DeviceToken, string Title, string Body, string? Data = null);

public record BulkSmsDto(IReadOnlyList<string> Recipients, string Message);

public record BulkPushDto(IReadOnlyList<string> DeviceTokens, string Title, string Body, string? Data = null);
