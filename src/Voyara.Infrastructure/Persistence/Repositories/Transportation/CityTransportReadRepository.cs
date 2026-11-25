 using Microsoft.EntityFrameworkCore;
using Voyara.Application.DTOs;
using Voyara.Application.DTOs.Transportation;
using Voyara.Application.Transportation;

namespace Voyara.Infrastructure.Persistence.Repositories.Transportation;

/// <summary>Read-only projections for the transportation module. Nothing returned here is a tracked entity.</summary>
public sealed class CityTransportReadRepository : ICityTransportReadRepository
{
    private readonly VoyaraDbContext _context;

    /// <summary>Creates the repository.</summary>
    public CityTransportReadRepository(VoyaraDbContext context) => _context = context;

    /// <inheritdoc />
    public async Task<PagedResult<CityTransportListItemDto>> SearchAsync(
        CityTransportSearchCriteria criteria,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        // The soft-delete query filter on CityTransport already hides deleted rows.
        var source = _context.CityTransports.AsNoTracking();

        if (criteria.CityId is { } cityId)
        {
            source = source.Where(c => c.CityId == cityId);
        }

        if (criteria.TransportTypeId is { } transportTypeId)
        {
            source = source.Where(c => c.TransportTypeId == transportTypeId);
        }

        if (criteria.DestinationId is { } destinationId)
        {
            source = source.Where(c => c.Destinations.Any(d => d.DestinationId == destinationId));
        }

        if (criteria.Currency is { } currency)
        {
            source = source.Where(c => c.Currency == currency);
        }

        if (criteria.MinCost is { } minCost)
        {
            source = source.Where(c => c.ApproxCost >= minCost);
        }

        if (criteria.MaxCost is { } maxCost)
        {
            source = source.Where(c => c.ApproxCost <= maxCost);
        }

        var total = await source.CountAsync(cancellationToken);

        var offset = (long)(page - 1) * pageSize;
        if (offset >= total)
        {
            return new PagedResult<CityTransportListItemDto>([], total, page, pageSize);
        }

        var items = await (
                from c in source
                join t in _context.TransportTypes.AsNoTracking() on c.TransportTypeId equals t.Id
                orderby t.Name, c.CityId, c.Id
                select new CityTransportListItemDto(
                    c.Id,
                    c.CityId,
                    t.Id,
                    t.Code,
                    t.Name,
                    c.Description,
                    c.ApproxCost,
                    c.Currency,
                    c.OperatingHours,
                    c.Routes.Count,
                    c.UpdatedAt ?? c.CreatedAt))
            .Skip((int)offset)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<CityTransportListItemDto>(items, total, page, pageSize);
    }

    /// <inheritdoc />
    public async Task<CityTransportDetailsDto?> GetDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Station.Location is stored through a value converter, so it is projected whole and
        // its coordinates are read in memory.
        var row = await (
                from c in _context.CityTransports.AsNoTracking().Where(c => c.Id == id)
                join t in _context.TransportTypes.AsNoTracking() on c.TransportTypeId equals t.Id
                select new
                {
                    c.Id,
                    c.CityId,
                    TransportTypeId = t.Id,
                    TransportTypeCode = t.Code,
                    TransportTypeName = t.Name,
                    c.Description,
                    c.ApproxCost,
                    c.Currency,
                    c.OperatingHours,
                    c.SafetyInfo,
                    LastUpdatedAt = c.UpdatedAt ?? c.CreatedAt,
                    Routes = c.Routes
                        .Select(r => new
                        {
                            r.Id,
                            r.Name,
                            r.Description,
                            r.Fare,
                            Stations = r.Stations.Select(s => new { s.Id, s.Name, s.Location }).ToList()
                        })
                        .ToList(),
                    DestinationIds = c.Destinations.Select(d => d.DestinationId).ToList()
                })
            .AsSplitQuery()
            .FirstOrDefaultAsync(cancellationToken);

        if (row is null)
        {
            return null;
        }

        var routes = row.Routes
            .Select(r => new TransportRouteDto(
                r.Id,
                r.Name,
                r.Description,
                r.Fare,
                r.Stations
                    .Select(s => new StationDto(s.Id, s.Name, s.Location.Latitude, s.Location.Longitude))
                    .ToList()))
            .ToList();

        return new CityTransportDetailsDto(
            row.Id,
            row.CityId,
            row.TransportTypeId,
            row.TransportTypeCode,
            row.TransportTypeName,
            row.Description,
            row.ApproxCost,
            row.Currency,
            row.OperatingHours,
            row.SafetyInfo,
            row.LastUpdatedAt,
            routes,
            row.DestinationIds);
    }
}
