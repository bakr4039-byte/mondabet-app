namespace Mondabet.Shared.Infrastructure.Storage;

public interface IStorageService
{
    /// <summary>Generates a pre-signed PUT URL for direct upload from client.</summary>
    Task<string> GetPresignedUploadUrlAsync(string bucket, string objectKey, int expirySeconds = 300, CancellationToken ct = default);

    /// <summary>Generates a pre-signed GET URL for secure download.</summary>
    Task<string> GetPresignedDownloadUrlAsync(string bucket, string objectKey, int expirySeconds = 3600, CancellationToken ct = default);

    /// <summary>Removes an object from storage.</summary>
    Task DeleteAsync(string bucket, string objectKey, CancellationToken ct = default);
}
