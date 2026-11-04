using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.LocalInfo;
using Voyara.Domain.Repositories.LocalInfo;

namespace Voyara.Infrastructure.Persistence.Repositories.LocalInfo;

public sealed class AwarenessArticleRepository : IAwarenessArticleRepository
{
    private readonly VoyaraDbContext _context;

    public AwarenessArticleRepository(VoyaraDbContext context)
    {
        _context = context;
    }

    public Task<AwarenessArticle?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _context.Set<AwarenessArticle>()
            .FirstOrDefaultAsync(
                article => article.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<AwarenessArticle>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<AwarenessArticle>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public void Add(AwarenessArticle article)
    {
        ArgumentNullException.ThrowIfNull(article);
        _context.Set<AwarenessArticle>().Add(article);
    }
}