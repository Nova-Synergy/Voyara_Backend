using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Culture_Rules;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Culture_Rules;

namespace Voyara.Infrastructure.Persistence.Repositories.Culture_Rules;

/// <summary>EF Core implementation of <see cref="ILocalRuleRepository"/>.</summary>
public sealed class LocalRuleRepository : ILocalRuleRepository
{
    private readonly VoyaraDbContext _context;

    /// <summary>Creates the repository.</summary>
    public LocalRuleRepository(VoyaraDbContext context) => _context = context;

    /// <inheritdoc />
    public Task<LocalRule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.LocalRules.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<LocalRule>> ListByScopesAsync(
        IReadOnlyCollection<RuleScope> scopes,
        CancellationToken cancellationToken = default)
    {
        if (scopes.Count == 0)
        {
            return [];
        }

        var countryIds = TargetIds(scopes, ScopeType.Country);
        var cityIds = TargetIds(scopes, ScopeType.City);
        var destinationIds = TargetIds(scopes, ScopeType.Destination);

        var results = await _context.LocalRules
            .AsNoTracking()
            .Where(r =>
                (r.Scope.Type == ScopeType.Country && countryIds.Contains(r.Scope.TargetId))
                || (r.Scope.Type == ScopeType.City && cityIds.Contains(r.Scope.TargetId))
                || (r.Scope.Type == ScopeType.Destination && destinationIds.Contains(r.Scope.TargetId)))
            .OrderBy(r => r.Type)
            .ToListAsync(cancellationToken);

        return results;
    }

    /// <inheritdoc />
    public void Add(LocalRule rule) => _context.LocalRules.Add(rule);

    private static List<string> TargetIds(IReadOnlyCollection<RuleScope> scopes, ScopeType type)
        => scopes.Where(s => s.Type == type).Select(s => s.TargetId).ToList();
}
