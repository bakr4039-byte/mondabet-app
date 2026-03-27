using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ClarificationAttachmentEntity = Mondabet.Clarification.Domain.Entities.ClarificationAttachment;

namespace Mondabet.Clarification.Infrastructure.Persistence.Configurations;

public class ClarificationAttachmentConfiguration : IEntityTypeConfiguration<ClarificationAttachmentEntity>
{
    public void Configure(EntityTypeBuilder<ClarificationAttachmentEntity> builder)
    {
        builder.ToTable("ClarificationAttachments");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.FileUrl).HasMaxLength(500).IsRequired();
        builder.Property(a => a.FileName).HasMaxLength(255).IsRequired();
        builder.HasIndex(a => a.ClarificationId);
    }
}
