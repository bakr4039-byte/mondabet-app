using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mondabet.Notification.Application.Interfaces;
using System.Text;
using System.Text.Json;

namespace Mondabet.Notification.Infrastructure.Adapters;

/// <summary>
/// Unifonic SMS adapter (Saudi provider).
/// Docs: https://unifonic.com/docs/sms-api
/// </summary>
public class UnifoncSmsService : ISmsService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly ILogger<UnifoncSmsService> _logger;

    public UnifoncSmsService(
        HttpClient http,
        IConfiguration config,
        ILogger<UnifoncSmsService> logger)
    {
        _http = http;
        _config = config;
        _logger = logger;
    }

    public async Task SendAsync(string to, string message, CancellationToken ct = default)
    {
        var payload = new
        {
            AppSid = _config["Unifonic:AppSid"],
            SenderID = _config["Unifonic:SenderId"],
            Body = message,
            Recipient = to,
        };
        await PostAsync(payload, ct);
    }

    public async Task SendBulkAsync(
        IReadOnlyList<string> recipients, string message, CancellationToken ct = default)
    {
        // Unifonic supports comma-separated recipients
        await SendAsync(string.Join(",", recipients), message, ct);
    }

    private async Task PostAsync(object payload, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var baseUrl = _config["Unifonic:BaseUrl"] ?? "https://api.unifonic.com/rest";
        var response = await _http.PostAsync($"{baseUrl}/Messages/Send", content, ct);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct);
            _logger.LogError("Unifonic SMS failed: {Status} {Body}", response.StatusCode, body);
        }
    }
}
