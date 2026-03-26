using Mondabet.Shared.Domain;

namespace Mondabet.Identity.Domain.Entities;

public class BiometricDevice : BaseEntity
{
    public Guid UserId { get; private set; }
    public string DeviceId { get; private set; } = default!;
    public string PublicKey { get; private set; } = default!;
    public DateTime RegisteredAt { get; private set; }
    public bool IsEnabled { get; private set; }

    protected BiometricDevice() { }

    public static BiometricDevice Register(Guid userId, string deviceId, string publicKey)
        => new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DeviceId = deviceId,
            PublicKey = publicKey,
            RegisteredAt = DateTime.UtcNow,
            IsEnabled = true
        };

    public void Disable() => IsEnabled = false;
}
