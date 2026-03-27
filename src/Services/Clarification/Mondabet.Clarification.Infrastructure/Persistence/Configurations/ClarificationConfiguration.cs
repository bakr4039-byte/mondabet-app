using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ClarificationEntity = Mondabet.Clarification.Domain.Entities.Clarification;
using ClarificationAttachmentEntity = Mondabet.Clarification.Domain.Entities.ClarificationAttachment;

namespace Mondabet.Clarification.Infrastructure.Persistence.Configurations;

public class ClarificationConfiguration : IEntityTypeConfiguration<ClarificationEntity>
{
    public void Configure(EntityTypeBuilder<ClarificationEntity> builder)
    {
        builder.ToTable("Clarifications");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Question).HasMaxLength(2000).IsRequired();
        builder.Property(c => c.ResponseText).HasMaxLength(2000);
        builder.HasIndex(c => c.EmployeeId);

        builder.HasMany<ClarificationAttachmentEntity>()
            .WithOne()
            .HasForeignKey(a => a.ClarificationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
