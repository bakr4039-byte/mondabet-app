using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mondabet.Employee.Application.Interfaces;
using Mondabet.Employee.Infrastructure.Persistence;
using Mondabet.Employee.Infrastructure.Repositories;
using Mondabet.Employee.Infrastructure.Services;
using Mondabet.Shared.Application;
using Mondabet.Shared.Infrastructure;
using Mondabet.Shared.Infrastructure.Audit;

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
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IExcelImportService, ExcelImportService>();

        // Audit logging (previously defined in Mondabet.Shared but never wired up anywhere) -
        // Employee/Department create/update/delete are now recorded via IAuditLogger.
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        services.AddAuditLogging(configuration.GetConnectionString("EmployeeDb")!);

        return services;
    }
}
