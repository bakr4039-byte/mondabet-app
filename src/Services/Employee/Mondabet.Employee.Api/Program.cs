using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Mondabet.Employee.Api.Endpoints;
using Mondabet.Employee.Infrastructure;
using Mondabet.Shared.Application.Behaviors;
using Mondabet.Shared.Infrastructure;
using Mondabet.Employee.Infrastructure.Persistence;
using Mondabet.Shared.Infrastructure.Audit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEmployeeInfrastructure(builder.Configuration);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(Mondabet.Employee.Application.Commands.CreateEmployee.CreateEmployeeCommand).Assembly));

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddValidatorsFromAssembly(
    typeof(Mondabet.Employee.Application.Commands.CreateEmployee.CreateEmployeeCommand).Assembly);

builder.Services.AddScoped<TenantProvider>();
builder.Services.AddScoped<Mondabet.Shared.Application.ITenantProvider>(
    sp => sp.GetRequiredService<TenantProvider>());

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var publicKeyPem = builder.Configuration["Jwt:PublicKeyPem"];
        if (!string.IsNullOrEmpty(publicKeyPem))
        {
            var rsa = System.Security.Cryptography.RSA.Create();
            rsa.ImportFromPem(publicKeyPem);
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.RsaSecurityKey(rsa),
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = builder.Configuration["Jwt:Audience"],
                ClockSkew = TimeSpan.Zero
            };
        }
        else
        {
            options.Authority = $"{builder.Configuration["Keycloak:BaseUrl"]}/realms/{builder.Configuration["Keycloak:Realm"]}";
            options.RequireHttpsMetadata = builder.Environment.IsProduction();
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };
        }
    });

builder.Services.AddAuthorization(options =>
{
    // Added alongside the new /api/v1/audit/employees/logs endpoint below - no existing
    // endpoint in this service referenced a named policy before this, so this is purely
    // additive and doesn't change any current endpoint's behavior.
    options.AddPolicy("CompanyAdmin", policy =>
        policy.RequireRole("CompanyAdmin", "SuperAdmin"));
});
builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("EmployeeDb")!);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<TenantMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapEmployeeEndpoints();
app.MapDepartmentEndpoints();

// Audit log viewer (new) - exposes the Employee/Department create/update/delete audit trail
// that CreateEmployeeCommandHandler etc. already write, for the portals' new Audit Log screen.
app.MapAuditLogEndpoints("/api/v1/audit/employees/logs");

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
    var db = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();
    await db.Database.EnsureCreatedAsync();

    var auditDb = scope.ServiceProvider.GetRequiredService<AuditDbContext>();
    await auditDb.EnsureAuditTableCreatedAsync();
}

app.Run();
