using Voyara.Domain.Entities.Culture_Rules;

namespace Voyara.Domain.Repositories.Culture_Rules;

/// <summary>Persistence contract for the LocalRule aggregate.</summary>
public interface ILocalRuleRepository
{
    /// <summary>Gets a rule with its translations, or <see langword="null"/>.</summary>
    Task<LocalRule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the rules that apply to any of the given scopes, for reading. Used to build the effective rules of a
    /// destination (destination scope + city scope + country scope). The rules are not tracked for changes.
    /// </summary>
    Task<IReadOnlyList<LocalRule>> ListByScopesAsync(
        IReadOnlyCollection<RuleScope> scopes,
        CancellationToken cancellationToken = default);

    /// <summary>Registers a new rule for insertion.</summary>
    void Add(LocalRule rule);
}
