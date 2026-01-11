using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrafficIncidentsOpitech.Application.Abstractions.Persistence;
using TrafficIncidentsOpitech.Infrastructure.Persistence;
using TrafficIncidentsOpitech.Infrastructure.Repositories;

namespace TrafficIncidentsOpitech.Infrastructure;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TrafficIncidentsDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped<ITrafficIncidentRepository, TrafficIncidentRepository>();
        services.AddScoped<ITrafficIncidentRepository, TrafficIncidentRepository>();
        services.AddScoped<ITrafficIncidentReadRepository, TrafficIncidentReadRepository>();


        return services;
    }
}
