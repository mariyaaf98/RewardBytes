using Microsoft.Extensions.DependencyInjection;
using FluentValidation ;
using AppWeaver.Mediator.Extensions;
using AppWeaver.Mediator.Default.Extensions;
using AppWeaver.Mediator.Behaviors;
using AppWeaver.EventBus.Extensions;
using AppWeaver.EventBus.Default.Extensions;
namespace BytesRewards.Service.Extensions;
public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(
                            this IServiceCollection services)
    {
        var applicationAssembly =
                typeof(ApplicationServiceRegistration).Assembly;

        // Register AppWeaver.Mediator with Default implementation and enable behaviors
        services.AddMediator(builder =>
        {
            builder.UseDefault()
                   .UseBehaviors()
                   .AddLoggingBehavior()
                   .AddFluentValidationBehavior();
        }, options => options.Provider = "Default");

        // Register handlers from application assembly
        services.AddAppWeaverMediatorHandlersFrom(
                        typeof(ApplicationServiceRegistration));

        // Register FluentValidation validators
        services.AddValidatorsFromAssembly(applicationAssembly);

        services.AddEventBus(builder =>
        {
            builder.UseDefaultEventBus(options =>
            {
                // Register all handlers in the same assembly as SendWelcomeEmailHandler
                options.RegisterServicesFromAssembly(typeof(ApplicationServiceRegistration).Assembly);
            });
        });

        return services;
    }
}