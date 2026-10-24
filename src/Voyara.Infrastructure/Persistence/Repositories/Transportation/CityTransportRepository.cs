using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Transportation;
using Voyara.Domain.Repositories.Transportation;

namespace Voyara.Infrastructure.Persistence.Repositories.Transportation;

/// <summary>EF Core implementation of <see cref="ICityTransportRepository"/>.</summary>
public sealed class CityTransportRepository : ICityTransportRepository
{
    private readonly VoyaraDbContext _context;

    /// <summary>Creates the repository.</summary>
    public CityTransportRepository(VoyaraDbContext context) => _context = context;

    /// <inheritdoc />
    public Task<CityTransport?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.CityTransports
            .AsSplitQuery()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task<bool> ExistsAsync(int cityId, int transportTypeId, CancellationToken cancellationToken = default)
        => _context.CityTransports
            .AnyAsync(c => c.CityId == cityId && c.TransportTypeId == transportTypeId, cancellationToken);

    /// <inheritdoc />
    public void Add(CityTransport cityTransport) => _context.CityTransports.Add(cityTransport);
}
