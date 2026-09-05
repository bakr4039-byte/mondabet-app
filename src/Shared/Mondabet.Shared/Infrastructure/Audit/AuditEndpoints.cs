using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Mondabet.Shared.Application;

namespace Mondabet.Shared.Infrastructure.Audit;

/// <summary>
/// Maps a single read-only "list audit logs" endpoint backed by AuditDbContext (which every
/// service that calls AddAuditLogging already has registered). This is a brand-new addition on
/// top of the pre-existing (but never exposed) audit logging infrastructure - it does not
/// change any existing endpoint or behavior.
///
/// Tenant scoping: a caller resolved to a specific tenant (the normal CompanyAdmin case - the
/// JWT carries a 'tid' claim, see TenantMiddleware) only ever sees that tenant's own audit
/// trail, regardless of what "?tenantId=" is passed. A caller with no resolved tenant (a
/// SuperAdmin token, which carries no 'tid') sees every tenant's entries and may narrow the
/// result with "?tenantId=".
/// </summary>
public static class AuditEndpoints
{
    public static void MapAuditLogEndpoints(
        this IEndpointRouteBuilder app, string path, string authorizationPolicy = "CompanyAdmin")
    {
        app.MapGet(path, async (
            string? action,
            string? entityType,
            Guid? tenantId,
            DateTimeOffset? from,
            DateTimeOffset? to,
            int? page,
            int? size,
            AuditDbContext db,
            ITenantProvider tenantProvider,
            CancellationToken ct) =>
        {
            // Nullable query params (rather than plain int with no default) so a request that
            // omits ?page=/?size= entirely still binds successfully instead of failing minimal
            // API's required-parameter check.
            var pageNumber = page is null or <= 0 ? 1 : page.Value;
            var pageSize = size is null or <= 0 or > 200 ? 20 : size.Value;

            var query = db.AuditLogs.AsNoTracking().AsQueryable();

            if (tenantProvider.TenantId is { } scopedTenantId)
                query = query.Where(a => a.TenantId == scopedTenantId.ToString());
            else if (tenantId is { } filterTenantId)
                query = query.Where(a => a.TenantId == filterTenantId.ToString());

            if (!string.IsNullOrWhiteSpace(action))
                query = query.Where(a => a.Action == action);
            if (!string.IsNullOrWhiteSpace(entityType))
                query = query.Where(a => a.EntityType == entityType);
            if (from is not null)
                query = query.Where(a => a.OccurredAt >= from);
            if (to is not null)
                query = query.Where(a => a.OccurredAt <= to);

            query = query.OrderByDescending(a => a.OccurredAt);

            var total = await query.CountAsync(ct);
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(ct);

            return Results.Ok(new AuditLogPageDto(items, total, pageNumber, pageSize));
        }).RequireAuthorization(authorizationPolicy);
    }
}

public record AuditLogPageDto(IReadOnlyList<AuditLog> Items, int Total, int Page, int Size);
