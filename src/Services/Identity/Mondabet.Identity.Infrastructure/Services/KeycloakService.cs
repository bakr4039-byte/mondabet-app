using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mondabet.Identity.Application.Interfaces;

namespace Mondabet.Identity.Infrastructure.Services;

public class KeycloakService : IKeycloakService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<KeycloakService> _logger;

    public KeycloakService(HttpClient httpClient, IConfiguration config, ILogger<KeycloakService> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    public async Task<bool> ValidateCredentialsAsync(string identifier, string password, CancellationToken ct = default)
    {
        var realm = _config["Keycloak:Realm"] ?? "mondabet";
        var clientId = _config["Keycloak:ClientId"] ?? "identity-svc";
        var clientSecret = _config["Keycloak:ClientSecret"] ?? string.Empty;

        var tokenEndpoint = $"{_config["Keycloak:BaseUrl"]}/realms/{realm}/protocol/openid-connect/token";

        var body = new FormUrlEncodedContent([
            new("grant_type", "password"),
            new("client_id", clientId),
            new("client_secret", clientSecret),
            new("username", identifier),
            new("password", password),
            new("scope", "openid")
        ]);

        try
        {
            var response = await _httpClient.PostAsync(tokenEndpoint, body, ct);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Keycloak credential validation failed for {Identifier}", identifier);
            return false;
        }
    }

    public async Task<IReadOnlyList<string>> GetUserRolesAsync(string keycloakSubject, CancellationToken ct = default)
    {
        var realm = _config["Keycloak:Realm"] ?? "mondabet";
        var adminToken = await GetAdminTokenAsync(ct);
        if (adminToken is null) return [];

        var url = $"{_config["Keycloak:BaseUrl"]}/admin/realms/{realm}/users/{keycloakSubject}/role-mappings/realm";

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);

        try
        {
            var roles = await _httpClient.GetFromJsonAsync<KeycloakRole[]>(url, ct);
            return roles?.Select(r => r.Name).ToList() ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get roles for user {Subject}", keycloakSubject);
            return [];
        }
    }

    private async Task<string?> GetAdminTokenAsync(CancellationToken ct)
    {
        var realm = "master";
        var url = $"{_config["Keycloak:BaseUrl"]}/realms/{realm}/protocol/openid-connect/token";

        var body = new FormUrlEncodedContent([
            new("grant_type", "client_credentials"),
            new("client_id", _config["Keycloak:AdminClientId"] ?? "admin-cli"),
            new("client_secret", _config["Keycloak:AdminClientSecret"] ?? string.Empty)
        ]);

        var response = await _httpClient.PostAsync(url, body, ct);
        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<TokenResponse>(ct);
        return result?.AccessToken;
    }

    private record KeycloakRole(string Id, string Name);
    private record TokenResponse(string AccessToken)
    {
        [System.Text.Json.Serialization.JsonPropertyName("access_token")]
        public string AccessToken { get; init; } = AccessToken;
    }
}
