using Minio;
using Minio.DataModel.Args;
using Microsoft.Extensions.Options;

namespace Mondabet.Shared.Infrastructure.Storage;

public sealed class MinioOptions
{
    public string Endpoint { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public bool UseSSL { get; set; } = true;
}

public sealed class MinioStorageService : IStorageService
{
    private readonly IMinioClient _client;

    public MinioStorageService(IOptions<MinioOptions> opts)
    {
        var o = opts.Value;
        _client = new MinioClient()
            .WithEndpoint(o.Endpoint)
            .WithCredentials(o.AccessKey, o.SecretKey)
            .WithSSL(o.UseSSL)
            .Build();
    }

    public async Task<string> GetPresignedUploadUrlAsync(
        string bucket, string objectKey, int expirySeconds = 300, CancellationToken ct = default)
    {
        await EnsureBucketAsync(bucket, ct);
        var args = new PresignedPutObjectArgs()
            .WithBucket(bucket)
            .WithObject(objectKey)
            .WithExpiry(expirySeconds);
        return await _client.PresignedPutObjectAsync(args).ConfigureAwait(false);
    }

    public async Task<string> GetPresignedDownloadUrlAsync(
        string bucket, string objectKey, int expirySeconds = 3600, CancellationToken ct = default)
    {
        var args = new PresignedGetObjectArgs()
            .WithBucket(bucket)
            .WithObject(objectKey)
            .WithExpiry(expirySeconds);
        return await _client.PresignedGetObjectAsync(args).ConfigureAwait(false);
    }

    public async Task DeleteAsync(string bucket, string objectKey, CancellationToken ct = default)
    {
        var args = new RemoveObjectArgs().WithBucket(bucket).WithObject(objectKey);
        await _client.RemoveObjectAsync(args, ct).ConfigureAwait(false);
    }

    private async Task EnsureBucketAsync(string bucket, CancellationToken ct)
    {
        var existsArgs = new BucketExistsArgs().WithBucket(bucket);
        bool exists = await _client.BucketExistsAsync(existsArgs, ct).ConfigureAwait(false);
        if (!exists)
        {
            var makeArgs = new MakeBucketArgs().WithBucket(bucket);
            await _client.MakeBucketAsync(makeArgs, ct).ConfigureAwait(false);
        }
    }
}
