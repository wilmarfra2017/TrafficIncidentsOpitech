using Microsoft.EntityFrameworkCore;
using TrafficIncidentsOpitech.Application.Abstractions.Persistence;
using TrafficIncidentsOpitech.Application.Common.Pagination;
using TrafficIncidentsOpitech.Application.Incidents.Queries.GetTrafficIncidents;
using TrafficIncidentsOpitech.Infrastructure.Persistence;

namespace TrafficIncidentsOpitech.Infrastructure.Repositories;

public sealed class TrafficIncidentReadRepository : ITrafficIncidentReadRepository
{
    private readonly TrafficIncidentsDbContext _db;

    public TrafficIncidentReadRepository(TrafficIncidentsDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<TrafficIncidentListItemDto>> SearchAsync(
        TrafficIncidentSearchCriteria criteria,
        CancellationToken ct = default)
    {
        var query = _db.TrafficIncidents
            .AsNoTracking()
            .Include(x => x.InvolvedVehicles)
            .AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(criteria.Department))
        {
            var dept = criteria.Department.Trim();
            query = query.Where(x => x.Department == dept);
        }

        if (criteria.From.HasValue)
            query = query.Where(x => x.OccurredAt >= criteria.From.Value);

        if (criteria.To.HasValue)
            query = query.Where(x => x.OccurredAt <= criteria.To.Value);

        query = query.OrderByDescending(x => x.OccurredAt);


        var totalCount = await query.CountAsync(ct);


        var page = criteria.Page <= 0 ? 1 : criteria.Page;
        var pageSize = criteria.PageSize <= 0 ? 10 : criteria.PageSize;
        pageSize = Math.Min(pageSize, 100);

        var skip = (page - 1) * pageSize;

        var items = await query
            .Skip(skip)
            .Take(pageSize)
            .Select(x => new TrafficIncidentListItemDto(
                x.Id,
                x.OccurredAt,
                x.Department,
                x.City,
                x.IncidentType,
                x.VictimCount,
                x.Description,
                x.InvolvedVehicles
                    .Select(v => new TrafficIncidentVehicleDto(v.VehicleType, v.Plate, v.Notes))
                    .ToList()
            ))
            .ToListAsync(ct);

        return new PagedResult<TrafficIncidentListItemDto>(items, totalCount, page, pageSize);
    }
}
