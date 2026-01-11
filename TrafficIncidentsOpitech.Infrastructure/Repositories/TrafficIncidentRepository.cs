using Microsoft.EntityFrameworkCore;
using TrafficIncidentsOpitech.Application.Abstractions.Persistence;
using TrafficIncidentsOpitech.Domain.Incidents;
using TrafficIncidentsOpitech.Infrastructure.Persistence;

namespace TrafficIncidentsOpitech.Infrastructure.Repositories;

public sealed class TrafficIncidentRepository : ITrafficIncidentRepository
{
    private readonly TrafficIncidentsDbContext _db;

    public TrafficIncidentRepository(TrafficIncidentsDbContext db)
    {
        _db = db;
    }

    public IUnitOfWork UnitOfWork => _db;

    public async Task AddAsync(TrafficIncidentAggregate incident, CancellationToken ct = default)
    {
        await _db.TrafficIncidents.AddAsync(incident, ct);
    }

    public async Task<TrafficIncidentAggregate?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.TrafficIncidents
            .Include(x => x.InvolvedVehicles)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }
}
