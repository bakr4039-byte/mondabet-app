using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mondabet.Shared.Application;
using Mondabet.Tenant.Application.Interfaces;
using Mondabet.Tenant.Infrastructure.Persistence;
using Mondabet.Tenant.Infrastructure.Repositories;

namespace Mondabet.Tenant.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddTenantInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<TenantDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("TenantDb"),
                sql => sql.MigrationsAssembly(typeof(TenantDbContext).Assembly.FullName)));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<TenantDbContext>());
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IPackageRepository, PackageRepository>();

        return services;
    }
}
