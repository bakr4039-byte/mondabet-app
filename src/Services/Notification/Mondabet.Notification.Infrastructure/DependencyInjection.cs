using Microsoft.Extensions.DependencyInjection;
using Mondabet.Notification.Application.Interfaces;
using Mondabet.Notification.Infrastructure.Adapters;

namespace Mondabet.Notification.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddNotificationInfrastructure(
        this IServiceCollection services)
    {
        services.AddHttpClient<ISmsService, UnifoncSmsService>();
        services.AddHttpClient<IPushService, FcmPushService>();
        return services;
    }
}
