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
        options.Authority = builder.Configuration["Keycloak:Authority"];
        options.Audience = builder.Configuration["Keycloak:Audience"];
        options.RequireHttpsMetadata = false;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CompanyAdmin", policy =>
        policy.RequireRole("CompanyAdmin", "SuperAdmin"));
    options.AddPolicy("SuperAdmin", policy =>
        policy.RequireRole("SuperAdmin"));
});

builder.Services.AddReportInfrastructure();
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(AttendanceReportQueryHandler).Assembly));

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
