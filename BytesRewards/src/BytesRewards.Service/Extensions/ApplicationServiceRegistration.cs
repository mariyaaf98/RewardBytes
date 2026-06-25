using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using AppWeaver.Mediator.Extensions;
using AppWeaver.Mediator.Default.Extensions;
using AppWeaver.Mediator.Behaviors;
using AppWeaver.EventBus.Extensions;
using AppWeaver.EventBus.Default.Extensions;
using BytesRewards.Service.Notifications.Services;

namespace BytesRewards.Service.Extensions;
public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(
                            this IServiceCollection services)
    {
        var applicationAssembly =
                typeof(ApplicationServiceRegistration).Assembly;

        services.AddMediator(builder =>
        {
            builder.UseDefault()
                   .UseBehaviors()
                   .AddLoggingBehavior()
                   .AddFluentValidationBehavior();
        }, options => options.Provider = "Default");

        services.AddAppWeaverMediatorHandlersFrom(
                        typeof(ApplicationServiceRegistration));

        services.AddValidatorsFromAssembly(applicationAssembly);

        services.AddEventBus(builder =>
        {
            builder.UseDefaultEventBus(options =>
            {
                options.RegisterServicesFromAssembly(typeof(ApplicationServiceRegistration).Assembly);
            });
        });

        // Notification writer — scoped so it shares the DbContext per request
        services.AddScoped<NotificationService>();

        return services;
    }
}