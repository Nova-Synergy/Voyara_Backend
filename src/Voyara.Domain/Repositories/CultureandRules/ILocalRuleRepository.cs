using Voyara.Domain.Entities.Culture_Rules;

namespace Voyara.Domain.Repositories.Culture_Rules;

/// <summary>Persistence contract for the <see cref="LocalRule"/> aggregate.</summary>
public interface ILocalRuleRepository
{
    /// <summary>Gets a rule with its translations, or <see langword="null"/>.</summary>
    Task<LocalRule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the rules that apply to any of the given scopes. Used to build the effective rules of a destination
    /// (destination scope + city scope + country scope).
    /// </summary>
    Task<IReadOnlyList<LocalRule>> ListByScopesAsync(
        IReadOnlyCollection<RuleScope> scopes,
        CancellationToken cancellationToken = default);

    /// <summary>Adds a new rule.</summary>
    Task AddAsync(LocalRule rule, CancellationToken cancellationToken = default);
}
