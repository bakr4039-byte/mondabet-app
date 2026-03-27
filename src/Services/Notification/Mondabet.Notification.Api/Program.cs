using Microsoft.AspNetCore.Authentication.JwtBearer;
using Mondabet.Notification.Api.Endpoints;
using Mondabet.Notification.Application.Commands.SendSms;
using Mondabet.Notification.Infrastructure;

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
});

builder.Services.AddNotificationInfrastructure();
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(SendSmsCommandHandler).Assembly));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapNotificationEndpoints();
app.MapHealthChecks("/health");

app.Run();
