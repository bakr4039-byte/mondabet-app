using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mondabet.Identity.Application.Interfaces;
using Mondabet.Identity.Infrastructure.Persistence;
using Mondabet.Identity.Infrastructure.Repositories;
using Mondabet.Identity.Infrastructure.Services;
using Mondabet.Shared.Application;
using Mondabet.Shared.Infrastructure.Audit;

namespace Mondabet.Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<IdentityDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("IdentityDb"),
                sql => sql.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName)));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<IdentityDbContext>());

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IBiometricDeviceRepository, BiometricDeviceRepository>();

        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IOtpService, OtpService>();
        services.AddScoped<IBiometricService, BiometricService>();
        services.AddScoped<INafathService, NafathService>();

        services.AddHttpClient<IKeycloakService, KeycloakService>(client =>
        {
            var baseUrl = configuration["Keycloak:BaseUrl"];
            if (!string.IsNullOrEmpty(baseUrl))
                client.BaseAddress = new Uri(baseUrl);
        });

        services.AddStackExchangeRedisCache(options =>
            options.Configuration = configuration.GetConnectionString("Redis"));

        // Audit logging (previously defined in Mondabet.Shared but never wired up anywhere) -
        // every successful login (via UserLoggedInAuditHandler) is now recorded.
        services.AddAuditLogging(configuration.GetConnectionString("IdentityDb")!);

        return services;
    }
}
