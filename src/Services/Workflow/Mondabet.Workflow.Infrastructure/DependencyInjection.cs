using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mondabet.Shared.Application;
using Mondabet.Workflow.Application.Interfaces;
using Mondabet.Workflow.Infrastructure.Consumers;
using Mondabet.Workflow.Infrastructure.Persistence;
using Mondabet.Workflow.Infrastructure.Repositories;

namespace Mondabet.Workflow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddWorkflowInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WorkflowDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<WorkflowDbContext>());
        services.AddScoped<IWorkflowDefinitionRepository, WorkflowDefinitionRepository>();
        services.AddScoped<IWorkflowInstanceRepository, WorkflowInstanceRepository>();

        services.AddMassTransit(bus =>
        {
            bus.AddConsumer<LeaveRequestedConsumer>();

            bus.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["RabbitMQ:Host"] ?? "rabbitmq", "/", h =>
                {
                    h.Username(configuration["RabbitMQ:Username"] ?? "guest");
                    h.Password(configuration["RabbitMQ:Password"] ?? "guest");
                });

                cfg.ReceiveEndpoint("workflow-leave-requested", e =>
                {
                    e.ConfigureConsumer<LeaveRequestedConsumer>(ctx);
                });
            });
        });

        return services;
    }
}
