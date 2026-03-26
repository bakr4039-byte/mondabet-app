using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Mondabet.Identity.Application.Interfaces;

namespace Mondabet.Identity.Infrastructure.Services;

public class BiometricService : IBiometricService
{
    private const string ChallengePrefix = "biometric:challenge:";
    private static readonly TimeSpan ChallengeTtl = TimeSpan.FromSeconds(15);

    private readonly IDistributedCache _cache;

    public BiometricService(IDistributedCache cache) => _cache = cache;

    public async Task<string> GenerateChallengeAsync(string deviceId, CancellationToken ct = default)
    {
        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        var challenge = Convert.ToBase64String(bytes);

        await _cache.SetStringAsync(
            $"{ChallengePrefix}{deviceId}",
            challenge,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ChallengeTtl },
            ct);

        return challenge;
    }

    public async Task<bool> VerifySignatureAsync(
        string deviceId,
        string challenge,
        string signedChallenge,
        string publicKey,
        CancellationToken ct = default)
    {
        var storedChallenge = await _cache.GetStringAsync($"{ChallengePrefix}{deviceId}", ct);
        if (storedChallenge is null) return false;

        await _cache.RemoveAsync($"{ChallengePrefix}{deviceId}", ct);

        try
        {
            using var rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKey), out _);

            var challengeBytes = Encoding.UTF8.GetBytes(storedChallenge);
            var signatureBytes = Convert.FromBase64String(signedChallenge);

            return rsa.VerifyData(
                challengeBytes,
                signatureBytes,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);
        }
        catch
        {
            return false;
        }
    }
}
