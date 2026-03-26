using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mondabet.Employee.Application.Interfaces;
using Mondabet.Employee.Infrastructure.Persistence;
using Mondabet.Employee.Infrastructure.Repositories;
using Mondabet.Employee.Infrastructure.Services;
using Mondabet.Shared.Application;

namespace Mondabet.Employee.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddEmployeeInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<EmployeeDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("EmployeeDb"),
                sql => sql.MigrationsAssembly(typeof(EmployeeDbContext).Assembly.FullName)));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<EmployeeDbContext>());
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IExcelImportService, ExcelImportService>();

        return services;
    }
}
