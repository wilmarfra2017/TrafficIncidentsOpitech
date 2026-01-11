using MediatR;
using TrafficIncidentsOpitech.Application.Common.Pagination;

namespace TrafficIncidentsOpitech.Application.Incidents.Queries.GetTrafficIncidents;

public sealed record GetTrafficIncidentsQuery(
    string? Department,
    DateTimeOffset? From,
    DateTimeOffset? To,
    int Page = 1,
    int PageSize = 10
) : IRequest<PagedResult<TrafficIncidentListItemDto>>;
