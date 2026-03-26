using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mondabet.Shared.Api;
using Mondabet.Tenant.Application.Commands.CreatePackage;
using Mondabet.Tenant.Application.Commands.CreateTenant;
using Mondabet.Tenant.Application.Commands.UpdateSubscription;
using Mondabet.Tenant.Application.Commands.UpdateTenant;
using Mondabet.Tenant.Application.DTOs;
using Mondabet.Tenant.Application.Queries.GetTenant;
using Mondabet.Tenant.Application.Queries.ListPackages;
using Mondabet.Tenant.Application.Queries.ListTenants;

namespace Mondabet.Tenant.Api.Endpoints;

public static class TenantEndpoints
{
    public static void MapTenantEndpoints(this IEndpointRouteBuilder app)
    {
        var tenants = app.MapGroup("/api/v1/tenants")
            .WithTags("Tenants")
            .RequireAuthorization("SuperAdmin");

        tenants.MapGet("/", async (
            [FromQuery] int page,
            [FromQuery] int size,
            [FromQuery] string? search,
            IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            var result = await mediator.Send(
                new ListTenantsQuery(page < 1 ? 1 : page, size < 1 ? 20 : size, search), ct);
            return result.ToApiResult(ctx);
        });

        tenants.MapGet("/{id:guid}", async (
            Guid id, IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetTenantQuery(id), ct);
            return result.ToApiResult(ctx);
        });

        tenants.MapPost("/", async (
            [FromBody] TenantCreateDto dto,
            IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            var result = await mediator.Send(new CreateTenantCommand(dto), ct);
            return result.ToApiResult(ctx);
        });

        tenants.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] TenantUpdateDto dto,
            IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            var result = await mediator.Send(new UpdateTenantCommand(id, dto), ct);
            return result.ToApiResult(ctx);
        });

        tenants.MapPut("/{id:guid}/subscription", async (
            Guid id,
            [FromBody] SubscriptionUpdateDto dto,
            IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            var result = await mediator.Send(new UpdateSubscriptionCommand(id, dto), ct);
            return result.ToApiResult(ctx);
        });

        // Packages
        var packages = app.MapGroup("/api/v1/packages")
            .WithTags("Packages")
            .RequireAuthorization("SuperAdmin");

        packages.MapGet("/", async (
            IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            var result = await mediator.Send(new ListPackagesQuery(), ct);
            return result.ToApiResult(ctx);
        });

        packages.MapPost("/", async (
            [FromBody] PackageCreateDto dto,
            IMediator mediator, HttpContext ctx, CancellationToken ct) =>
        {
            var result = await mediator.Send(new CreatePackageCommand(dto), ct);
            return result.ToApiResult(ctx);
        });
    }
}
