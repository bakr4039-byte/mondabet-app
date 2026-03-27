using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace Mondabet.Shared.Infrastructure.Security;

public sealed class EncryptionOptions
{
    /// <summary>32-byte (256-bit) Base64-encoded key. Store in Key Vault / sealed secret.</summary>
    public string Key { get; set; } = string.Empty;
}

/// <summary>
/// AES-256-CBC encryption for PII fields (Iqama numbers, national IDs).
/// Output is Base64(IV + CipherText) — the IV is randomly generated per encryption
/// so the same plaintext produces different ciphertext each time.
/// </summary>
public sealed class AesEncryptionService
{
    private readonly byte[] _key;

    public AesEncryptionService(IOptions<EncryptionOptions> opts)
    {
        var decoded = Convert.FromBase64String(opts.Value.Key);
        if (decoded.Length != 32)
            throw new InvalidOperationException("AES key must be exactly 32 bytes (256-bit).");
        _key = decoded;
    }

    public string Encrypt(string plaintext)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();
        using var encryptor = aes.CreateEncryptor();
        var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        var cipherBytes = encryptor.TransformFinalBlock(plaintextBytes, 0, plaintextBytes.Length);
        // Prepend IV so Decrypt can recover it
        var result = new byte[aes.IV.Length + cipherBytes.Length];
        aes.IV.CopyTo(result, 0);
        cipherBytes.CopyTo(result, aes.IV.Length);
        return Convert.ToBase64String(result);
    }

    public string Decrypt(string ciphertext)
    {
        var fullBytes = Convert.FromBase64String(ciphertext);
        using var aes = Aes.Create();
        aes.Key = _key;
        var iv = new byte[16];
        var cipher = new byte[fullBytes.Length - 16];
        Array.Copy(fullBytes, 0, iv, 0, 16);
        Array.Copy(fullBytes, 16, cipher, 0, cipher.Length);
        aes.IV = iv;
        using var decryptor = aes.CreateDecryptor();
        var plainBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
        return Encoding.UTF8.GetString(plainBytes);
    }
}
