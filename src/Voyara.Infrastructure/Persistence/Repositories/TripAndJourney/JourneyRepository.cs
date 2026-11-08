using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.TripAndJourney;
using Voyara.Domain.Repositories.TripAndJourney;

namespace Voyara.Infrastructure.Persistence.Repositories.TripAndJourney;

/// <summary>
/// EF Core implementation of <see cref="IJourneyRepository"/>.
/// </summary>
public sealed class JourneyRepository : IJourneyRepository
{
    private readonly VoyaraDbContext _context;

    /// <summary>
    /// Initializes a new instance of <see cref="JourneyRepository"/>.
    /// </summary>
    /// <param name="context">The database context.</param>
    public JourneyRepository(VoyaraDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<Journey?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Journey>()
            .Include(j => j.Days)
                .ThenInclude(d => d.Items)
            .AsSplitQuery()
            .FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public void Add(Journey journey)
    {
        _context.Set<Journey>().Add(journey);
    }

    /// <inheritdoc />
    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Set<Journey>().AnyAsync(j => j.Id == id, cancellationToken);
    }
}
