namespace Mondabet.Identity.Application.Interfaces;

public interface IBiometricService
{
    Task<string> GenerateChallengeAsync(string deviceId, CancellationToken ct = default);
    Task<bool> VerifySignatureAsync(string deviceId, string challenge, string signedChallenge, string publicKey, CancellationToken ct = default);
}
