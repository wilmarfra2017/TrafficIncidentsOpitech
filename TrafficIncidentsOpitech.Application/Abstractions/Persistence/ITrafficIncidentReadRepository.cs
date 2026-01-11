using TrafficIncidentsOpitech.Application.Common.Pagination;
using TrafficIncidentsOpitech.Application.Incidents.Queries.GetTrafficIncidents;

namespace TrafficIncidentsOpitech.Application.Abstractions.Persistence;

public interface ITrafficIncidentReadRepository
{
    Task<PagedResult<TrafficIncidentListItemDto>> SearchAsync(
        TrafficIncidentSearchCriteria criteria,
        CancellationToken ct = default);
}
