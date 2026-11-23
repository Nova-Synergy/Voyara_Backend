using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.TripAndJourney;
using Voyara.Domain.Repositories.TripAndJourney;

namespace Voyara.Infrastructure.Persistence.Repositories.TripAndJourney;

/// <summary>
/// EF Core implementation of <see cref="ITripRepository"/>.
/// </summary>
public sealed class TripRepository : ITripRepository
{
    private readonly VoyaraDbContext _context;

    /// <summary>
    /// Initializes a new instance of <see cref="TripRepository"/>.
    /// </summary>
    /// <param name="context">The database context.</param>
    public TripRepository(VoyaraDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<Trip?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Trip>()
            .Include(t => t.Shares)
            .Include(t => t.Journey)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Trip>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Trip>()
            .Include(t => t.Shares)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.StartDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Trip?> GetByShareTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Trip>()
            .Include(t => t.Shares)
            .Include(t => t.Journey)
            .FirstOrDefaultAsync(t => t.Shares.Any(s => s.Token == token && s.RevokedAt == null && (s.ExpiresAt == null || s.ExpiresAt > DateTime.UtcNow)), cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Set<Trip>().AnyAsync(t => t.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public void Add(Trip trip)
    {
        _context.Set<Trip>().Add(trip);
    }

    /// <inheritdoc />
    public void Update(Trip trip)
    {
        _context.Set<Trip>().Update(trip);
    }

    /// <inheritdoc />
    public void Delete(Trip trip)
    {
        _context.Set<Trip>().Remove(trip);
    }
}
