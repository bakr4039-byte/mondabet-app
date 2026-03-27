using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mondabet.Notification.Application.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Mondabet.Notification.Infrastructure.Adapters;

/// <summary>
/// Firebase Cloud Messaging (FCM) v1 push adapter.
/// Uses the HTTP v1 API with OAuth2 service account token.
/// </summary>
public class FcmPushService : IPushService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly ILogger<FcmPushService> _logger;

    public FcmPushService(
        HttpClient http,
        IConfiguration config,
        ILogger<FcmPushService> logger)
    {
        _http = http;
        _config = config;
        _logger = logger;
    }

    public async Task SendAsync(
        string deviceToken, string title, string body,
        string? data = null, CancellationToken ct = default)
    {
        var projectId = _config["Fcm:ProjectId"];
        var serverKey = _config["Fcm:ServerKey"];

        var payload = new
        {
            message = new
            {
                token = deviceToken,
                notification = new { title, body },
                data = data is not null ? JsonSerializer.Deserialize<Dictionary<string, string>>(data) : null,
            }
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", serverKey);

        var url = $"https://fcm.googleapis.com/v1/projects/{projectId}/messages:send";
        var response = await _http.PostAsync(url, content, ct);

        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync(ct);
            _logger.LogError("FCM push failed: {Status} {Body}", response.StatusCode, responseBody);
        }
    }

    public async Task SendBulkAsync(
        IReadOnlyList<string> deviceTokens, string title, string body,
        string? data = null, CancellationToken ct = default)
    {
        // FCM v1 does not support multicast in the same way — send sequentially
        // For production: use FCM batch or topic subscriptions
        var tasks = deviceTokens.Select(token => SendAsync(token, title, body, data, ct));
        await Task.WhenAll(tasks);
    }
}
