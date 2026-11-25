using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Geography;
using Voyara.Domain.Repositories.Geography;

namespace Voyara.Infrastructure.Persistence.Repositories.Geography;

/// <summary>EF Core implementation of <see cref="ICityRepository"/> (read-side lookups).</summary>
public sealed class CityRepository : ICityRepository
{
    private readonly VoyaraDbContext _context;
    public async Task<Country?> GetByCityIdAsync(int cityId, CancellationToken cancellationToken = default)
        => await _context.Countries
            .Include(c => c.Cities)
            .FirstOrDefaultAsync(c => c.Cities.Any(city => city.Id == cityId), cancellationToken);
}
