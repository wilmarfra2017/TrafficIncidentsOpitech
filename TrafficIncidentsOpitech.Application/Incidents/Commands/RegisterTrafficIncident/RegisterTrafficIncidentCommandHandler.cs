using MediatR;
using TrafficIncidentsOpitech.Application.Abstractions.Persistence;
using TrafficIncidentsOpitech.Domain.Incidents;

namespace TrafficIncidentsOpitech.Application.Incidents.Commands.RegisterTrafficIncident;

public sealed class RegisterTrafficIncidentCommandHandler
    : IRequestHandler<RegisterTrafficIncidentCommand, Guid>
{
    private readonly ITrafficIncidentRepository _repository;

    public RegisterTrafficIncidentCommandHandler(ITrafficIncidentRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(RegisterTrafficIncidentCommand request, CancellationToken ct)
    {        
        var id = Guid.NewGuid();

        var incident = new TrafficIncidentAggregate(
            id: id,
            occurredAt: request.OccurredAt,
            department: request.Department,
            city: request.City,
            incidentType: request.IncidentType,
            victimCount: request.VictimCount,
            description: request.Description
        );
        
        if (request.Vehicles is null || request.Vehicles.Count == 0)
            throw new ArgumentException("Se requiere al menos un vehículo involucrado.", nameof(request.Vehicles));

        foreach (var v in request.Vehicles)
            incident.AddVehicle(v.VehicleType, v.Plate, v.Notes);

        await _repository.AddAsync(incident, ct);
        await _repository.UnitOfWork.SaveChangesAsync(ct);

        return id;
    }
}
