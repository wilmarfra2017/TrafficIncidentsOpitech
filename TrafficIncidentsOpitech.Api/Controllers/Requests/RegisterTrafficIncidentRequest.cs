using TrafficIncidentsOpitech.Domain.Enums;

namespace TrafficIncidentsOpitech.Api.Controllers.Requests;

public sealed record RegisterTrafficIncidentRequest(
    DateTimeOffset OccurredAt,
    string Department,
    string City,
    IncidentType IncidentType,
    int VictimCount,
    string? Description,
    List<RegisterTrafficIncidentVehicleRequest>? Vehicles
);

public sealed record RegisterTrafficIncidentVehicleRequest(
    VehicleType VehicleType,
    string? Plate,
    string? Notes
);
