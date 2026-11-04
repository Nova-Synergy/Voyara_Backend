using Microsoft.EntityFrameworkCore;
using LocalRuleEntity = Voyara.Domain.Entities.LocalInfo.LocalRule;
using Voyara.Domain.Repositories.LocalInfo;

namespace Voyara.Infrastructure.Persistence.Repositories.LocalInfo;

public sealed class LocalRuleRepository : ILocalRuleRepository
{
    private readonly VoyaraDbContext _context;

    public LocalRuleRepository(VoyaraDbContext context)
    {
        _context = context;
    }

    public Task<LocalRuleEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _context.Set<LocalRuleEntity>()
            .Include(rule => rule.Translations)
            .FirstOrDefaultAsync(
                rule => rule.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<LocalRuleEntity>> GetByScopeAsync(
        Guid scopeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<LocalRuleEntity>()
            .AsNoTracking()
            .Include(rule => rule.Translations)
            .Where(rule => rule.ScopeId == scopeId)
            .ToListAsync(cancellationToken);
    }

    public void Add(LocalRuleEntity rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        _context.Set<LocalRuleEntity>().Add(rule);
    }
}