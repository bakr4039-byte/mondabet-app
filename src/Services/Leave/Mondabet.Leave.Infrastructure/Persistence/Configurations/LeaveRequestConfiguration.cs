using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mondabet.Leave.Domain.Entities;

namespace Mondabet.Leave.Infrastructure.Persistence.Configurations;

public class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
{
    public void Configure(EntityTypeBuilder<LeaveRequest> builder)
    {
        builder.ToTable("LeaveRequests");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Reason).HasMaxLength(1000).IsRequired();
        builder.Property(l => l.RejectionReason).HasMaxLength(500);
        builder.Property(l => l.ApprovalComment).HasMaxLength(500);

        builder.HasIndex(l => new { l.EmployeeId, l.Status })
               .HasDatabaseName("IX_LeaveRequests_Employee_Status");
    }
}
