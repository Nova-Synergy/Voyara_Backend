using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Geography;
using Voyara.Domain.Repositories.Geography;

namespace Voyara.Infrastructure.Persistence.Repositories.Geography;

public sealed class CityRepository : ICityRepository
{
    private readonly VoyaraDbContext _context;

    public CityRepository(VoyaraDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<City>> GetAllAsync(
        int? countryId = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Cities.AsNoTracking().AsQueryable();

        if (countryId.HasValue)
            query = query.Where(c => c.CountryId == countryId.Value);

        if (isActive.HasValue)
            query = query.Where(c => c.IsActive == isActive.Value);

        return await query
            .OrderBy(c => c.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<City?> GetByIdWithCountryAsync(
        int id,
        CancellationToken cancellationToken = default)
        => await _context.Cities
            .AsNoTracking()
            .Include(c => c.Country)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
}