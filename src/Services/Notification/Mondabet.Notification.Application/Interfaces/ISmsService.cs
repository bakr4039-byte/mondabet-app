namespace Mondabet.Notification.Application.Interfaces;

public interface ISmsService
{
    Task SendAsync(string to, string message, CancellationToken ct = default);
    Task SendBulkAsync(IReadOnlyList<string> recipients, string message, CancellationToken ct = default);
}
