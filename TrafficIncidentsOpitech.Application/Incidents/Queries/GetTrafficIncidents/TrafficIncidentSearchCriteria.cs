namespace TrafficIncidentsOpitech.Application.Incidents.Queries.GetTrafficIncidents;

public sealed record TrafficIncidentSearchCriteria(
    string? Department,
    DateTimeOffset? From,
    DateTimeOffset? To,
    int Page,
    int PageSize
);
