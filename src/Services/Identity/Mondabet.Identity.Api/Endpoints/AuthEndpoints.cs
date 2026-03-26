using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mondabet.Identity.Application.Commands.BiometricChallenge;
using Mondabet.Identity.Application.Commands.BiometricVerify;
using Mondabet.Identity.Application.Commands.Login;
using Mondabet.Identity.Application.Commands.Logout;
using Mondabet.Identity.Application.Commands.MfaVerify;
using Mondabet.Identity.Application.Commands.NafathInitiate;
using Mondabet.Identity.Application.Commands.NafathVerify;
using Mondabet.Identity.Application.Commands.Refresh;
using Mondabet.Shared.Api;
using System.Security.Claims;

namespace Mondabet.Identity.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/auth").WithTags("Auth");

        group.MapPost("/login", async (
            [FromBody] LoginRequest body,
            IMediator mediator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(
                new LoginCommand(body.Identifier, body.Password, body.TenantCode), ct);
            return result.ToApiResult(ctx);
        });

        group.MapPost("/mfa/verify", async (
            [FromBody] MfaVerifyRequest body,
            IMediator mediator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new MfaVerifyCommand(body.SessionToken, body.Otp), ct);
            return result.ToApiResult(ctx);
        });

        group.MapPost("/biometric/challenge", async (
            [FromBody] BiometricChallengeRequest body,
            IMediator mediator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new BiometricChallengeCommand(body.DeviceId), ct);
            return result.ToApiResult(ctx);
        });

        group.MapPost("/biometric/verify", async (
            [FromBody] BiometricVerifyRequest body,
            IMediator mediator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(
                new BiometricVerifyCommand(body.DeviceId, body.SignedChallenge), ct);
            return result.ToApiResult(ctx);
        });

        group.MapPost("/nafath/initiate", async (
            [FromBody] NafathInitiateRequest body,
            IMediator mediator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new NafathInitiateCommand(body.IqamaNumber), ct);
            return result.ToApiResult(ctx);
        });

        group.MapPost("/nafath/verify", async (
            [FromBody] NafathVerifyRequest body,
            IMediator mediator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(
                new NafathVerifyCommand(body.TransactionId, body.IqamaNumber), ct);
            return result.ToApiResult(ctx);
        });

        group.MapPost("/refresh", async (
            [FromBody] RefreshRequest body,
            IMediator mediator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new RefreshTokenCommand(body.RefreshToken), ct);
            return result.ToApiResult(ctx);
        });

        group.MapPost("/logout", async (
            IMediator mediator,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var sub = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? ctx.User.FindFirstValue("sub");

            if (!Guid.TryParse(sub, out var userId))
                return Results.Unauthorized();

            var refreshToken = ctx.Request.Headers.Authorization
                .FirstOrDefault()?.Replace("Bearer ", string.Empty);

            await mediator.Send(new LogoutCommand(userId, refreshToken), ct);
            return Results.NoContent();
        }).RequireAuthorization();
    }
}

// Request records
public record LoginRequest(string Identifier, string Password, string TenantCode);
public record MfaVerifyRequest(string SessionToken, string Otp);
public record BiometricChallengeRequest(string DeviceId);
public record BiometricVerifyRequest(string DeviceId, string SignedChallenge);
public record NafathInitiateRequest(string IqamaNumber);
public record NafathVerifyRequest(string TransactionId, string IqamaNumber);
public record RefreshRequest(string RefreshToken);
