using System.Security.Cryptography;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mondabet.Identity.Application.Interfaces;

namespace Mondabet.Identity.Infrastructure.Services;

public class OtpService : IOtpService
{
    private const string OtpPrefix = "otp:";
    private const string SessionPrefix = "session:";

    private readonly IDistributedCache _cache;
    private readonly IConfiguration _config;
    private readonly ILogger<OtpService> _logger;

    public OtpService(IDistributedCache cache, IConfiguration config, ILogger<OtpService> logger)
    {
        _cache = cache;
        _config = config;
        _logger = logger;
    }

    public async Task<string> GenerateAndSendOtpAsync(string mobileNumber, CancellationToken ct = default)
    {
        var otp = GenerateOtp();
        var key = $"{OtpPrefix}{mobileNumber}";
        var ttlMinutes = int.Parse(_config["Otp:TtlMinutes"] ?? "5");

        await _cache.SetStringAsync(key, otp, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(ttlMinutes)
        }, ct);

        // In production, call Unifonic SMS API here
        _logger.LogInformation("OTP for {Mobile}: {Otp} (TTL {Ttl}m)", MaskMobile(mobileNumber), otp, ttlMinutes);

        return mobileNumber; // return mobile as session key reference
    }

    public async Task<bool> VerifyOtpAsync(string sessionToken, string otp, CancellationToken ct = default)
    {
        var userId = await GetUserIdFromSessionAsync(sessionToken, ct);
        if (userId is null) return false;

        var key = $"{OtpPrefix}{sessionToken}";
        var stored = await _cache.GetStringAsync(key, ct);

        if (stored is null || stored != otp) return false;

        await _cache.RemoveAsync(key, ct);
        return true;
    }

    public async Task StoreSessionAsync(string sessionToken, Guid userId, CancellationToken ct = default)
    {
        var sessionKey = $"{SessionPrefix}{sessionToken}";
        var otpKey = $"{OtpPrefix}{sessionToken}";
        var otp = GenerateOtp();
        var ttlMinutes = int.Parse(_config["Otp:TtlMinutes"] ?? "5");

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(ttlMinutes)
        };

        await _cache.SetStringAsync(sessionKey, userId.ToString(), options, ct);
        await _cache.SetStringAsync(otpKey, otp, options, ct);

        _logger.LogInformation("MFA session {Session} created for user {UserId}, OTP: {Otp}",
            sessionToken[..8], userId, otp);
    }

    public async Task<Guid?> GetUserIdFromSessionAsync(string sessionToken, CancellationToken ct = default)
    {
        var key = $"{SessionPrefix}{sessionToken}";
        var value = await _cache.GetStringAsync(key, ct);
        return Guid.TryParse(value, out var id) ? id : null;
    }

    private static string GenerateOtp()
    {
        var bytes = new byte[4];
        RandomNumberGenerator.Fill(bytes);
        var value = BitConverter.ToUInt32(bytes, 0) % 1_000_000;
        return value.ToString("D6");
    }

    private static string MaskMobile(string mobile)
        => mobile.Length > 7
            ? mobile[..4] + new string('*', mobile.Length - 7) + mobile[^3..]
            : mobile;
}
