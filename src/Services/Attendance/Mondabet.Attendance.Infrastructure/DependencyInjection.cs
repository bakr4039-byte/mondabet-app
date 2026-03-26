using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mondabet.Attendance.Application.Interfaces;
using Mondabet.Attendance.Infrastructure.Persistence;
using Mondabet.Attendance.Infrastructure.Repositories;
using Mondabet.Shared.Application;
using Mondabet.Shared.Infrastructure;

namespace Mondabet.Attendance.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAttendanceInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AttendanceDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AttendanceDbContext>());
        services.AddScoped<IShiftRepository, ShiftRepository>();
        services.AddScoped<IAttendanceRepository, AttendanceRepository>();

        return services;
    }
}
