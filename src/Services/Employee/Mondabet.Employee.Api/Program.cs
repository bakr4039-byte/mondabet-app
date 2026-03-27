using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Mondabet.Employee.Api.Endpoints;
using Mondabet.Employee.Infrastructure;
using Mondabet.Shared.Application.Behaviors;
using Mondabet.Shared.Infrastructure;
using Mondabet.Employee.Infrastructure.Persistence;

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
        options.Authority = $"{builder.Configuration["Keycloak:BaseUrl"]}/realms/{builder.Configuration["Keycloak:Realm"]}";
        options.RequireHttpsMetadata = builder.Environment.IsProduction();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("EmployeeDb")!);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<TenantMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapEmployeeEndpoints();
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
}

app.Run();
