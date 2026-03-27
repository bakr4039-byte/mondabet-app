using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mondabet.Leave.Application.Interfaces;
using Mondabet.Leave.Infrastructure.Persistence;
using Mondabet.Leave.Infrastructure.Repositories;
using Mondabet.Shared.Application;

namespace Mondabet.Leave.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddLeaveInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<LeaveDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<LeaveDbContext>());
        services.AddScoped<ILeaveRepository, LeaveRepository>();

        return services;
    }
}
