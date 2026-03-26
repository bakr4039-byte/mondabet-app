namespace Mondabet.Identity.Application.Interfaces;

public interface IOtpService
{
    Task<string> GenerateAndSendOtpAsync(string mobileNumber, CancellationToken ct = default);
    Task<bool> VerifyOtpAsync(string sessionToken, string otp, CancellationToken ct = default);
    Task StoreSessionAsync(string sessionToken, Guid userId, CancellationToken ct = default);
    Task<Guid?> GetUserIdFromSessionAsync(string sessionToken, CancellationToken ct = default);
}
