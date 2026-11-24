
using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Community;
using Voyara.Domain.Repositories.Community;

namespace Voyara.Infrastructure.Persistence.Repositories.Community;

/// <summary>
/// EF Core implementation of the favorite repository.
/// </summary>
public sealed class FavoriteRepository : IFavoriteRepository
{
    private readonly VoyaraDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="FavoriteRepository"/> class.
    /// </summary>
    /// <param name="context">The database context used for community persistence.</param>
    public FavoriteRepository(VoyaraDbContext context)
    {
        _context = context;
    }

    public Task<Favorite?> GetByUserAndDestinationAsync(
        Guid userId,
        Guid destinationId,
        CancellationToken cancellationToken = default)
    {
        // Include deleted records so callers can restore an existing favorite.
        return _context.Favorites
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                favorite =>
                    favorite.UserId == userId &&
                    favorite.DestinationId == destinationId,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Favorite>> GetByUserIdAsync(
        Guid userId,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        return await _context.Favorites
            .AsNoTracking()
            .Where(favorite => favorite.UserId == userId)
            .OrderByDescending(favorite => favorite.CreatedAt)
            .ThenByDescending(favorite => favorite.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> CountByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return _context.Favorites
            .CountAsync(
                favorite => favorite.UserId == userId,
                cancellationToken);
    }

    /// <inheritdoc />
    public void Add(Favorite favorite)
    {
        _context.Favorites.Add(favorite);
    }
}
