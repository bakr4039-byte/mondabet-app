using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mondabet.Identity.Domain.Entities;

namespace Mondabet.Identity.Infrastructure.Persistence.Configurations;

public class BiometricDeviceConfiguration : IEntityTypeConfiguration<BiometricDevice>
{
    public void Configure(EntityTypeBuilder<BiometricDevice> builder)
    {
        builder.ToTable("BiometricDevices", "dbo");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.DeviceId).IsRequired().HasMaxLength(200);
        builder.Property(b => b.PublicKey).IsRequired().HasMaxLength(4000);

        builder.HasIndex(b => b.DeviceId).IsUnique();
        builder.HasIndex(b => b.UserId);
    }
}
