using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mondabet.Employee.Infrastructure.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Domain.Entities.Employee>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Employee> builder)
    {
        builder.ToTable("Employees");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.FullNameAr).IsRequired().HasMaxLength(200);
        builder.Property(e => e.FullNameEn).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Iqama).IsRequired().HasMaxLength(10);
        builder.Property(e => e.JobTitle).IsRequired().HasMaxLength(200);
        builder.Property(e => e.MobileNumber).IsRequired().HasMaxLength(20);
        builder.Property(e => e.Email).IsRequired().HasMaxLength(200);
        builder.HasIndex(e => e.Iqama).IsUnique();

        builder.Property(e => e.EmployeeNumber).HasMaxLength(50);
        builder.Property(e => e.PinCode).HasMaxLength(20);
        builder.Property(e => e.GamificationBadge).HasMaxLength(100);
        builder.Property(e => e.BaseSalary).HasColumnType("decimal(18,2)");
        builder.Property(e => e.HourlyRate).HasColumnType("decimal(18,2)");
    }
}
