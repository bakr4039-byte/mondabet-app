using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mondabet.Leave.Application.Interfaces;
using Mondabet.Leave.Infrastructure.Persistence;
using Mondabet.Leave.Infrastructure.Repositories;
using Mondabet.Leave.Infrastructure.Services;
using Mondabet.Shared.Application;
using Mondabet.Shared.Infrastructure;
using Mondabet.Shared.Infrastructure.Audit;

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

        // Audit logging (previously defined in Mondabet.Shared but never wired up anywhere) -
        // LeaveApprovedEvent/LeaveRejectedEvent already existed and were already raised on
        // every approval/rejection, but had no handler at all until now.
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        services.AddAuditLogging(configuration.GetConnectionString("DefaultConnection")!);

        // Substitute-candidate suggestion (cross-service call to Employee).
        services.AddHttpContextAccessor();
        services.AddHttpClient("EmployeeApi", client =>
        {
            client.BaseAddress = new Uri(configuration["Services:EmployeeBaseUrl"]!);
        });
        services.AddScoped<IEmployeeLookupService, EmployeeLookupService>();

        return services;
    }
}
