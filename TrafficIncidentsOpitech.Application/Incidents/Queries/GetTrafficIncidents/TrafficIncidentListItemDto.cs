using TrafficIncidentsOpitech.Domain.Enums;

namespace TrafficIncidentsOpitech.Application.Incidents.Queries.GetTrafficIncidents;

public sealed record TrafficIncidentListItemDto(
    Guid Id,
    DateTimeOffset OccurredAt,
    string Department,
    string City,
    IncidentType IncidentType,
    int VictimCount,
    string? Description,
    IReadOnlyList<TrafficIncidentVehicleDto> Vehicles
);

public sealed record TrafficIncidentVehicleDto(
    VehicleType VehicleType,
    string? Plate,
    string? Notes
);
