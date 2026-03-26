using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mondabet.Tenant.Domain.Entities;

namespace Mondabet.Tenant.Infrastructure.Persistence.Configurations;

public class PackageConfiguration : IEntityTypeConfiguration<Package>
{
    public void Configure(EntityTypeBuilder<Package> builder)
    {
        builder.ToTable("Packages", "dbo");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.PriceMonthly).HasColumnType("decimal(10,2)");
        builder.Property(p => p.FeaturesJson).HasColumnType("nvarchar(max)");
    }
}
