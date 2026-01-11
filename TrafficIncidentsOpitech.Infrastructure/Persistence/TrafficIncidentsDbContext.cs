using Microsoft.EntityFrameworkCore;
using TrafficIncidentsOpitech.Application.Abstractions.Persistence;
using TrafficIncidentsOpitech.Domain.Incidents;

namespace TrafficIncidentsOpitech.Infrastructure.Persistence;

public class TrafficIncidentsDbContext : DbContext, IUnitOfWork
{
    public TrafficIncidentsDbContext(DbContextOptions<TrafficIncidentsDbContext> options)
        : base(options)
    {
    }

    public DbSet<TrafficIncidentAggregate> TrafficIncidents => Set<TrafficIncidentAggregate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TrafficIncidentsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
