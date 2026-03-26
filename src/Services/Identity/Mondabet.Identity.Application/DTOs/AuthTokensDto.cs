namespace Mondabet.Identity.Application.DTOs;

public record AuthTokensDto(string AccessToken, string RefreshToken);

public record LoginResponse(
    string? AccessToken,
    string? RefreshToken,
    bool MfaRequired,
    string? SessionToken);

public record MfaVerifyResponse(string AccessToken, string RefreshToken);

public record BiometricChallengeResponse(string Challenge);

public record NafathTransactionResponse(string TransactionId);
