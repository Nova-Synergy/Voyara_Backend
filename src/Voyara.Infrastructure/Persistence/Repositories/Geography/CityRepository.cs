using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Geography;
using Voyara.Domain.Repositories.Geography;

namespace Voyara.Infrastructure.Persistence.Repositories.Geography;

/// <summary>EF Core implementation of <see cref="ICityRepository"/> (read-side lookups).</summary>
public sealed class CityRepository : ICityRepository
{
    private readonly VoyaraDbContext _context;

    /// <summary>Initializes a new instance of the <see cref="CityRepository"/> class.</summary>
    /// <param name="context">The application database context.</param>
    public CityRepository(VoyaraDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public Task<City?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Cities
            .AsNoTracking()
            .Include(city => city.Country)
            .FirstOrDefaultAsync(city => city.Id == id, cancellationToken);
    }
}
