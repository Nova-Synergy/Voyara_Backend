using Voyara.Domain.Entities.LocalInfo;

namespace Voyara.Domain.Repositories.LocalInfo;

/// <summary>
/// Persistence contract for the AwarenessArticle aggregate.
/// </summary>
public interface IAwarenessArticleRepository
{
    Task<AwarenessArticle?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AwarenessArticle>> GetAllAsync(
        CancellationToken cancellationToken = default);

    void Add(AwarenessArticle article);
}