using TrafficIncidentsOpitech.Domain.Incidents;

namespace TrafficIncidentsOpitech.Application.Abstractions.Persistence;

public interface ITrafficIncidentRepository
{
    IUnitOfWork UnitOfWork { get; }
    Task AddAsync(TrafficIncidentAggregate incident, CancellationToken ct = default);
    Task<TrafficIncidentAggregate?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
