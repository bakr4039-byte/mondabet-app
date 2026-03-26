namespace Mondabet.Identity.Application.Interfaces;

public interface IKeycloakService
{
    Task<bool> ValidateCredentialsAsync(string identifier, string password, CancellationToken ct = default);
    Task<IReadOnlyList<string>> GetUserRolesAsync(string keycloakSubject, CancellationToken ct = default);
}
