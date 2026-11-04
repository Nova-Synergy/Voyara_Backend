using LocalRuleEntity = Voyara.Domain.Entities.LocalInfo.LocalRule;

namespace Voyara.Domain.Repositories.LocalInfo;

/// <summary>
/// Persistence contract for LocalInfo LocalRule.
/// </summary>
public interface ILocalRuleRepository
{
    Task<LocalRuleEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LocalRuleEntity>> GetByScopeAsync(
        Guid scopeId,
        CancellationToken cancellationToken = default);

    void Add(LocalRuleEntity rule);
}