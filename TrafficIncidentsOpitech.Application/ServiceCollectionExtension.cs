using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TrafficIncidentsOpitech.Application.Common.Behaviors;

namespace TrafficIncidentsOpitech.Application;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtension).Assembly));
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtension).Assembly);        
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }
}
