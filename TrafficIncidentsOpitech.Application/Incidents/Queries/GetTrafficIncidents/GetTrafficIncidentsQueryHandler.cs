using MediatR;
using TrafficIncidentsOpitech.Application.Abstractions.Persistence;
using TrafficIncidentsOpitech.Application.Common.Pagination;

namespace TrafficIncidentsOpitech.Application.Incidents.Queries.GetTrafficIncidents;

public sealed class GetTrafficIncidentsQueryHandler
    : IRequestHandler<GetTrafficIncidentsQuery, PagedResult<TrafficIncidentListItemDto>>
{
    private readonly ITrafficIncidentReadRepository _readRepository;

    public GetTrafficIncidentsQueryHandler(ITrafficIncidentReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<PagedResult<TrafficIncidentListItemDto>> Handle(GetTrafficIncidentsQuery request, CancellationToken ct)
    {        
        if (request.Page <= 0) throw new ArgumentOutOfRangeException(nameof(request.Page), "La página debe ser >= 1.");
        if (request.PageSize <= 0) throw new ArgumentOutOfRangeException(nameof(request.PageSize), "El tamaño de la página debe ser >= 1.");
        if (request.PageSize > 100) throw new ArgumentOutOfRangeException(nameof(request.PageSize), "El tamaño de la página debe ser <= 100.");

        if (request.From is not null && request.To is not null && request.From > request.To)
            throw new ArgumentException("Desde no puede ser mayor que Hasta.");

        var criteria = new TrafficIncidentSearchCriteria(
            Department: request.Department,
            From: request.From,
            To: request.To,
            Page: request.Page,
            PageSize: request.PageSize
        );

        return await _readRepository.SearchAsync(criteria, ct);
    }
}
