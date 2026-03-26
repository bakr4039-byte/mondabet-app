using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mondabet.Identity.Application.Interfaces;

namespace Mondabet.Identity.Infrastructure.Services;

public class NafathService : INafathService
{
    private const string NafathPrefix = "nafath:";

    private readonly IDistributedCache _cache;
    private readonly IConfiguration _config;
    private readonly ILogger<NafathService> _logger;

    public NafathService(IDistributedCache cache, IConfiguration config, ILogger<NafathService> logger)
    {
        _cache = cache;
        _config = config;
        _logger = logger;
    }

    public async Task<string> InitiateAsync(string iqamaNumber, CancellationToken ct = default)
    {
        // TODO: Call actual Nafath API (ZATCA/NIC Saudi integration)
        // Stub: generate a transaction ID and cache it
        var transactionId = Guid.NewGuid().ToString("N");

        await _cache.SetStringAsync(
            $"{NafathPrefix}{transactionId}",
            iqamaNumber,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            },
            ct);

        _logger.LogInformation("Nafath initiated for Iqama ending ...{IqamaSuffix}, txId={TxId}",
            iqamaNumber.Length > 4 ? iqamaNumber[^4..] : "****",
            transactionId);

        return transactionId;
    }

    public async Task<bool> VerifyAsync(string transactionId, CancellationToken ct = default)
    {
        // TODO: Call actual Nafath verification API
        // Stub: check that transaction exists in cache
        var stored = await _cache.GetStringAsync($"{NafathPrefix}{transactionId}", ct);
        if (stored is null) return false;

        await _cache.RemoveAsync($"{NafathPrefix}{transactionId}", ct);
        return true;
    }
}
