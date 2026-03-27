using Microsoft.Extensions.DependencyInjection;
using Mondabet.Report.Application.Interfaces;
using Mondabet.Report.Infrastructure.Services;

namespace Mondabet.Report.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddReportInfrastructure(
        this IServiceCollection services)
    {
        services.AddSingleton<IReportGenerator, ReportGenerator>();
        services.AddScoped<IAttendanceDataService, StubAttendanceDataService>();
        services.AddScoped<ITenantDataService, StubTenantDataService>();
        return services;
    }
}
