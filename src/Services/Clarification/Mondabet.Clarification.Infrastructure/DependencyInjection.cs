using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mondabet.Clarification.Application.Interfaces;
using Mondabet.Clarification.Infrastructure.Persistence;
using Mondabet.Clarification.Infrastructure.Repositories;
using Mondabet.Shared.Application;

namespace Mondabet.Clarification.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddClarificationInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ClarificationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ClarificationDbContext>());
        services.AddScoped<IClarificationRepository, ClarificationRepository>();

        return services;
    }
}
