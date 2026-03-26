namespace Mondabet.Identity.Application.Interfaces;

public interface INafathService
{
    Task<string> InitiateAsync(string iqamaNumber, CancellationToken ct = default);
    Task<bool> VerifyAsync(string transactionId, CancellationToken ct = default);
}
