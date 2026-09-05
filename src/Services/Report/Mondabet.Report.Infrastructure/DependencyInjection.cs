using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mondabet.Report.Application.Interfaces;
using Mondabet.Report.Infrastructure.Services;

namespace Mondabet.Report.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddReportInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IReportGenerator, ReportGenerator>();
        services.AddHttpContextAccessor();

        services.AddHttpClient("AttendanceApi", client =>
        {
            client.BaseAddress = new Uri(configuration["Services:AttendanceBaseUrl"]!);
        });
        services.AddHttpClient("EmployeeApi", client =>
        {
            client.BaseAddress = new Uri(configuration["Services:EmployeeBaseUrl"]!);
        });
        services.AddHttpClient("TenantApi", client =>
        {
            client.BaseAddress = new Uri(configuration["Services:TenantBaseUrl"]!);
        });

        services.AddScoped<IAttendanceDataService, AttendanceDataService>();
        services.AddScoped<ITenantDataService, TenantDataService>();
        return services;
    }
}
