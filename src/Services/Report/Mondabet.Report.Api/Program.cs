using Microsoft.AspNetCore.Authentication.JwtBearer;
using Mondabet.Report.Api.Endpoints;
using Mondabet.Report.Application.Queries.AttendanceReport;
using Mondabet.Report.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var publicKeyPem = builder.Configuration["Jwt:PublicKeyPem"];
        if (!string.IsNullOrEmpty(publicKeyPem))
        {
            var rsa = System.Security.Cryptography.RSA.Create();
            rsa.ImportFromPem(publicKeyPem);
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
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
            options.Authority = builder.Configuration["Keycloak:Authority"];
            options.Audience = builder.Configuration["Keycloak:Audience"];
            options.RequireHttpsMetadata = false;
        }
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CompanyAdmin", policy =>
        policy.RequireRole("CompanyAdmin", "SuperAdmin"));
    options.AddPolicy("SuperAdmin", policy =>
        policy.RequireRole("SuperAdmin"));
});

builder.Services.AddReportInfrastructure(builder.Configuration);
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(AttendanceReportQueryHandler).Assembly));

builder.Services.AddHealthChecks();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapReportEndpoints();
app.MapHealthChecks("/health");

app.Run();
