using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Geography;
using Voyara.Domain.Repositories.Geography;

namespace Voyara.Infrastructure.Persistence.Repositories.Geography;

/// <summary>EF Core implementation of <see cref="ICityRepository"/> (read-side lookups).</summary>
public sealed class CityRepository : ICityRepository
{
    private readonly VoyaraDbContext _context;

    public CityRepository(VoyaraDbContext context)
    {
        _context = context;
    }

    public Task<bool> ActiveCityExistsAsync(
        int cityId,
        CancellationToken cancellationToken)
    {
        return _context.Cities
            .AsNoTracking()
            .AnyAsync(
                city => city.Id == cityId && city.IsActive,
                cancellationToken);
    }
}   
