using Microsoft.AspNetCore.Authentication.JwtBearer;
using Mondabet.Shared.Application;
using Mondabet.Shared.Infrastructure;
using Mondabet.Workflow.Api.Endpoints;
using Mondabet.Workflow.Application.Commands.CreateWorkflow;
using Mondabet.Workflow.Infrastructure;

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

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<TenantProvider>();
builder.Services.AddScoped<ITenantProvider>(sp => sp.GetRequiredService<TenantProvider>());

builder.Services.AddWorkflowInfrastructure(builder.Configuration);
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateWorkflowCommandHandler).Assembly));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<TenantMiddleware>();

app.MapWorkflowEndpoints();
app.MapHealthChecks("/health");

app.Run();
