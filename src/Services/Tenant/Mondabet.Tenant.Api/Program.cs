using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Mondabet.Shared.Application.Behaviors;
using Mondabet.Shared.Infrastructure;
using Mondabet.Tenant.Api.Endpoints;
using Mondabet.Tenant.Infrastructure;
using Mondabet.Tenant.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTenantInfrastructure(builder.Configuration);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(Mondabet.Tenant.Application.Commands.CreateTenant.CreateTenantCommand).Assembly));

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddValidatorsFromAssembly(
    typeof(Mondabet.Tenant.Application.Commands.CreateTenant.CreateTenantCommandValidator).Assembly);

builder.Services.AddScoped<TenantProvider>();
builder.Services.AddScoped<Mondabet.Shared.Application.ITenantProvider>(
    sp => sp.GetRequiredService<TenantProvider>());

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"{builder.Configuration["Keycloak:BaseUrl"]}/realms/{builder.Configuration["Keycloak:Realm"]}";
        options.RequireHttpsMetadata = builder.Environment.IsProduction();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SuperAdmin", policy =>
        policy.RequireRole("SuperAdmin"));
    options.AddPolicy("CompanyAdmin", policy =>
        policy.RequireRole("SuperAdmin", "CompanyAdmin"));
});

builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("TenantDb")!);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<TenantMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapTenantEndpoints();
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
    var db = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
    await db.Database.EnsureCreatedAsync();
}

app.Run();
