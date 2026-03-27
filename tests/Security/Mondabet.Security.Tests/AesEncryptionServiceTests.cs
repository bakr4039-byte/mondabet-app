using FluentAssertions;
using Microsoft.Extensions.Options;
using Mondabet.Shared.Infrastructure.Security;
using Xunit;

namespace Mondabet.Security.Tests;

public class AesEncryptionServiceTests
{
    private static AesEncryptionService CreateService()
    {
        // 32 bytes (256-bit) test key
        var key = Convert.ToBase64String(new byte[32] {
            1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,
            17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32
        });
        var opts = Options.Create(new EncryptionOptions { Key = key });
        return new AesEncryptionService(opts);
    }

    [Fact]
    public void Encrypt_ThenDecrypt_ShouldReturnOriginalPlaintext()
    {
        var svc = CreateService();
        const string iqama = "1234567890";

        var cipher = svc.Encrypt(iqama);
        var result = svc.Decrypt(cipher);

        result.Should().Be(iqama);
    }

    [Fact]
    public void Encrypt_SamePlaintext_ProducesDifferentCiphertext()
    {
        var svc = CreateService();
        const string iqama = "9876543210";

        var cipher1 = svc.Encrypt(iqama);
        var cipher2 = svc.Encrypt(iqama);

        // Random IV means different output each time
        cipher1.Should().NotBe(cipher2);
    }

    [Fact]
    public void Decrypt_InvalidBase64_ShouldThrow()
    {
        var svc = CreateService();
        var act = () => svc.Decrypt("not-valid-base64!!!");
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Constructor_InvalidKeyLength_ShouldThrow()
    {
        var shortKey = Convert.ToBase64String(new byte[16]);
        var opts = Options.Create(new EncryptionOptions { Key = shortKey });
        var act = () => new AesEncryptionService(opts);
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*32 bytes*");
    }

    [Theory]
    [InlineData("1234567890")]     // Standard Iqama
    [InlineData("2098765432")]     // Resident Iqama (starts with 2)
    [InlineData("hello world")]    // Arbitrary text
    public void RoundTrip_VariousInputs(string input)
    {
        var svc = CreateService();
        svc.Decrypt(svc.Encrypt(input)).Should().Be(input);
    }
}
