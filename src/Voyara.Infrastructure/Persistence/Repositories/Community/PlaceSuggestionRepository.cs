
using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Community;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Community;

namespace Voyara.Infrastructure.Persistence.Repositories.Community;

/// <summary>
/// EF Core implementation of the place-suggestion repository.
/// </summary>
public sealed class PlaceSuggestionRepository
    : IPlaceSuggestionRepository
{
    private readonly VoyaraDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaceSuggestionRepository"/> class.
    /// </summary>
    /// <param name="context">The database context used for community persistence.</param>
    public PlaceSuggestionRepository(VoyaraDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public Task<PlaceSuggestion?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _context.PlaceSuggestions
            .Include(suggestion => suggestion.Photos)
            .FirstOrDefaultAsync(
                suggestion => suggestion.Id == id,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PlaceSuggestion>> GetByUserAsync(
        Guid userId,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        return await _context.PlaceSuggestions
            .AsNoTracking()
            .Include(suggestion => suggestion.Photos)
            .Where(suggestion => suggestion.UserId == userId)
            .OrderByDescending(suggestion => suggestion.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> CountByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return _context.PlaceSuggestions
            .CountAsync(
                suggestion => suggestion.UserId == userId,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PlaceSuggestion>> GetByStatusAsync(
        SuggestionStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _context.PlaceSuggestions
            .AsNoTracking()
            .Include(suggestion => suggestion.Photos)
            .Where(suggestion => suggestion.Status == status)
            .OrderBy(suggestion => suggestion.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public void Add(PlaceSuggestion suggestion)
    {
        _context.PlaceSuggestions.Add(suggestion);
    }
}
