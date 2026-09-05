using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Mondabet.Identity.Api.Endpoints;
using Mondabet.Identity.Infrastructure;
using Mondabet.Shared.Application.Behaviors;
using Mondabet.Shared.Infrastructure;
using System.Security.Cryptography;
using Mondabet.Identity.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Infrastructure (DB, Redis, Services)
builder.Services.AddIdentityInfrastructure(builder.Configuration);

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(Mondabet.Identity.Application.Commands.Login.LoginCommand).Assembly));

// FluentValidation pipeline
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddValidatorsFromAssembly(
    typeof(Mondabet.Identity.Application.Commands.Login.LoginCommandValidator).Assembly);

// Tenant resolution
builder.Services.AddScoped<TenantProvider>();
builder.Services.AddScoped<Mondabet.Shared.Application.ITenantProvider>(
    sp => sp.GetRequiredService<TenantProvider>());

// JWT authentication (RS256 – public key from Keycloak JWKS or config)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var publicKeyPem = builder.Configuration["Jwt:PublicKeyPem"];
        if (!string.IsNullOrEmpty(publicKeyPem))
        {
            var rsa = RSA.Create();
            rsa.ImportFromPem(publicKeyPem);
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new RsaSecurityKey(rsa),
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = builder.Configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        }
        else
        {
            // Fall back to Keycloak JWKS endpoint for token validation
            options.Authority = $"{builder.Configuration["Keycloak:BaseUrl"]}/realms/{builder.Configuration["Keycloak:Realm"]}";
            options.RequireHttpsMetadata = builder.Environment.IsProduction();
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };
        }
    });

builder.Services.AddAuthorization();

// Health checks
builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("IdentityDb")!)
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<TenantMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();

app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Auto-migrate on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    await db.Database.EnsureCreatedAsync();
}

app.Run();
