using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mondabet.Tenant.Infrastructure.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Domain.Entities.Tenant>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Tenant> builder)
    {
        builder.ToTable("Tenants", "dbo");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Code).IsRequired().HasMaxLength(50);
        builder.Property(t => t.CompanyName).IsRequired().HasMaxLength(200);
        builder.Property(t => t.LogoUrl).HasMaxLength(500);
        builder.Property(t => t.PrimaryColor).IsRequired().HasMaxLength(7);
        builder.Property(t => t.SecondaryColor).IsRequired().HasMaxLength(7);
        builder.Property(t => t.Slogan).HasMaxLength(300);
        builder.Property(t => t.Address).HasMaxLength(500);
        builder.Property(t => t.NationalAddress).HasMaxLength(200);
        builder.Property(t => t.BankAccount).HasMaxLength(100);
        builder.Property(t => t.ZakatNumber).HasMaxLength(50);
        builder.Property(t => t.AdminEmail).IsRequired().HasMaxLength(200);
        builder.Property(t => t.AdminMobile).IsRequired().HasMaxLength(20);
        builder.HasIndex(t => t.Code).IsUnique();
    }
}
