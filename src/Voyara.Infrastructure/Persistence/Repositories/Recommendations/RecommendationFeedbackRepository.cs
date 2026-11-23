using Microsoft.EntityFrameworkCore;
using Voyara.Application.Abstractions;
using Voyara.Domain.Entities.AiAssistant;

namespace Voyara.Infrastructure.Persistence.Repositories.Recommendations;

/// <summary>
/// EF Core implementation of <see cref="IRecommendationFeedbackRepository"/>.
/// </summary>
public sealed class RecommendationFeedbackRepository : IRecommendationFeedbackRepository
{
    private readonly VoyaraDbContext _context;

    /// <summary>
    /// Initializes a new instance of <see cref="RecommendationFeedbackRepository"/>.
    /// </summary>
    /// <param name="context">The database context.</param>
    public RecommendationFeedbackRepository(VoyaraDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public void Add(RecommendationLog log)
    {
        _context.Set<RecommendationLog>().Add(log);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RecommendationLog>> GetByDestinationIdAsync(Guid destinationId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<RecommendationLog>()
            .Where(r => r.DestinationId == destinationId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
