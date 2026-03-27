using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mondabet.Leave.Domain.Entities;

namespace Mondabet.Leave.Infrastructure.Persistence.Configurations;

public class LeaveAttachmentConfiguration : IEntityTypeConfiguration<LeaveAttachment>
{
    public void Configure(EntityTypeBuilder<LeaveAttachment> builder)
    {
        builder.ToTable("LeaveAttachments");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.FileUrl).HasMaxLength(500).IsRequired();
        builder.Property(a => a.FileName).HasMaxLength(200).IsRequired();
    }
}
