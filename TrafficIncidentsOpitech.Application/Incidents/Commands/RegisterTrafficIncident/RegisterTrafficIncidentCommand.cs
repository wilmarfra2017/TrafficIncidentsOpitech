using MediatR;
using TrafficIncidentsOpitech.Domain.Enums;

namespace TrafficIncidentsOpitech.Application.Incidents.Commands.RegisterTrafficIncident;

public sealed record RegisterTrafficIncidentCommand(
    DateTimeOffset OccurredAt,
    string Department,
    string City,
    IncidentType IncidentType,
    int VictimCount,
    string? Description,
    IReadOnlyCollection<RegisterTrafficIncidentVehicleDto> Vehicles
) : IRequest<Guid>;

public sealed record RegisterTrafficIncidentVehicleDto(
    VehicleType VehicleType,
    string? Plate,
    string? Notes
);
