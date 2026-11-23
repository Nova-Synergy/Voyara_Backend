
using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Community;
using Voyara.Domain.Repositories.Community;

namespace Voyara.Infrastructure.Persistence.Repositories.Community;

/// <summary>
/// EF Core implementation of the review repository.
/// </summary>
public sealed class ReviewRepository : IReviewRepository
{
    private readonly VoyaraDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReviewRepository"/> class.
    /// </summary>
    /// <param name="context">The database context used for community persistence.</param>
    public ReviewRepository(VoyaraDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public Task<Review?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _context.Reviews
            .Include(review => review.Photos)
            .FirstOrDefaultAsync(
                review => review.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Review>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .AsNoTracking()
            .Include(review => review.Photos)
            .Where(review => review.UserId == userId)
            .OrderByDescending(review => review.VisitDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public void Add(Review review)
    {
        _context.Reviews.Add(review);
    }
}
