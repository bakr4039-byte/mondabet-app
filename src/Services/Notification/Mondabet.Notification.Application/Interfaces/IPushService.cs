namespace Mondabet.Notification.Application.Interfaces;

public interface IPushService
{
    Task SendAsync(string deviceToken, string title, string body, string? data = null, CancellationToken ct = default);
    Task SendBulkAsync(IReadOnlyList<string> deviceTokens, string title, string body, string? data = null, CancellationToken ct = default);
}
