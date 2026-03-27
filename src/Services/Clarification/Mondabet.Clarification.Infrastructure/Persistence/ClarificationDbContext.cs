using MediatR;
using Microsoft.EntityFrameworkCore;
using Mondabet.Shared.Application;
using Mondabet.Shared.Infrastructure;
using ClarificationEntity = Mondabet.Clarification.Domain.Entities.Clarification;
using ClarificationAttachmentEntity = Mondabet.Clarification.Domain.Entities.ClarificationAttachment;

namespace Mondabet.Clarification.Infrastructure.Persistence;

/// <summary>
/// Clarifications live in per-tenant schema.
/// </summary>
public class ClarificationDbContext : BaseDbContext
{
    public DbSet<ClarificationEntity> Clarifications => Set<ClarificationEntity>();
    public DbSet<ClarificationAttachmentEntity> ClarificationAttachments => Set<ClarificationAttachmentEntity>();

    public ClarificationDbContext(
        DbContextOptions<ClarificationDbContext> options,
        ITenantProvider tenantProvider,
        IMediator mediator)
        : base(options, tenantProvider, mediator) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClarificationDbContext).Assembly);
    }
}
