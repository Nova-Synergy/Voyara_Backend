using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Culture_Rules;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Culture_Rules;

namespace Voyara.Infrastructure.Persistence.Repositories.Culture_Rules;

public sealed class LocalRuleRepository : ILocalRuleRepository
{
    private readonly VoyaraDbContext _context;

    public LocalRuleRepository(VoyaraDbContext context)
    {
        _context = context;
    }

    public Task<LocalRule?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _context.Set<LocalRule>()
            .Include(r => r.Translations)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<LocalRule>> ListByScopesAsync(
        IReadOnlyCollection<RuleScope> scopes,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(scopes);

        if (scopes.Count == 0)
            return [];

        var countryIds = TargetIds(scopes, ScopeType.Country);
        var cityIds = TargetIds(scopes, ScopeType.City);
        var destinationIds = TargetIds(scopes, ScopeType.Destination);

        return await _context.Set<LocalRule>()
            .AsNoTracking()
            .Include(r => r.Translations)
            .Where(r =>
                (r.Scope.Type == ScopeType.Country &&
                 countryIds.Contains(r.Scope.TargetId))
                ||
                (r.Scope.Type == ScopeType.City &&
                 cityIds.Contains(r.Scope.TargetId))
                ||
                (r.Scope.Type == ScopeType.Destination &&
                 destinationIds.Contains(r.Scope.TargetId)))
            .OrderBy(r => r.Type)
            .ToListAsync(cancellationToken);
    }

    public void Add(LocalRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        _context.Set<LocalRule>().Add(rule);
    }

    private static List<string> TargetIds(
        IReadOnlyCollection<RuleScope> scopes,
        ScopeType type)
    {
        return scopes
            .Where(s => s.Type == type)
            .Select(s => s.TargetId)
            .ToList();
    }
}