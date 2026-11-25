using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Geography;
using Voyara.Domain.Repositories.Geography;

namespace Voyara.Infrastructure.Persistence.Repositories.Geography;

public sealed class CountryRepository : ICountryRepository
{
    private readonly VoyaraDbContext _context;

    public CountryRepository(VoyaraDbContext context)
    {
        _context = context;
    }

    public async Task<Country?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Countries
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<Country?> GetByCityIdAsync(int cityId, CancellationToken cancellationToken = default)
        => await _context.Countries
            .Include(c => c.Cities)
            .FirstOrDefaultAsync(c => c.Cities.Any(city => city.Id == cityId), cancellationToken);

    public async Task<bool> ExistsByIsoCodeAsync(string isoCode, CancellationToken cancellationToken = default)
    {
        var normalized = isoCode.Trim().ToUpperInvariant();
        return await _context.Countries
            .AnyAsync(c => c.IsCode == normalized, cancellationToken);
    }

    public async Task<IReadOnlyList<Country>> GetAllAsync(
        bool? isActive = null,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Countries.AsNoTracking().AsQueryable();

        if (isActive.HasValue)
            query = query.Where(c => c.IsActive == isActive.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToUpperInvariant();
            query = query.Where(c =>
                c.IsCode.Contains(term) ||
                c.DefaultCurrency.Contains(term) ||
                c.TimeZone.ToUpper().Contains(term));
        }

        return await query
            .OrderBy(c => c.IsCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<Country?> GetByIdWithCitiesAsync(
        int id,
        CancellationToken cancellationToken = default)
        => await _context.Countries
            .AsNoTracking()
            .Include(c => c.Cities)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public void Add(Country country)
        => _context.Countries.Add(country);

    public void Update(Country country)
        => _context.Countries.Update(country);
}