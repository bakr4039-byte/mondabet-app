using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mondabet.Attendance.Domain.Entities;

namespace Mondabet.Attendance.Infrastructure.Persistence.Configurations;

public class AttendanceRecordConfiguration : IEntityTypeConfiguration<AttendanceRecord>
{
    public void Configure(EntityTypeBuilder<AttendanceRecord> builder)
    {
        builder.ToTable("Attendance");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.DeviceId).HasMaxLength(100).IsRequired();

        // Critical index per db-schema.md
        builder.HasIndex(a => new { a.EmployeeId, a.CheckInTime })
               .HasDatabaseName("IX_Attendance_Employee_Date");
    }
}
