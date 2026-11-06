using Voyara.Domain.Enums;
using Voyara.SharedKernel.Results;
using Voyara.SharedKernel.ValueObjects;

namespace Voyara.Domain.Entities.Culture_Rules;

/// <summary>
/// Value object that says where a <see cref="LocalRule"/> applies: a country, a city or a destination.
/// </summary>
/// <remarks>
/// Countries, cities and destinations are all identified by <c>Guid</c>. The factory methods keep
/// that type safety, and the identifier is stored as text. The target is referenced by Id only.
/// </remarks>
public sealed class RuleScope : ValueObject
{
    /// <summary>The level the rule applies to.</summary>
    public ScopeType Type { get; private set; }

    /// <summary>Identifier of the targeted country, city or destination, as text.</summary>
    public string TargetId { get; private set; } = string.Empty;

    private RuleScope()
    {
    }

    private RuleScope(ScopeType type, string targetId)
    {
        Type = type;
        TargetId = targetId;
    }

    /// <summary>Creates a scope that targets a country.</summary>
    public static Result<RuleScope> ForCountry(Guid countryId)
    {
        if (countryId == Guid.Empty)
        {
            return Result.Failure<RuleScope>(LocalRuleErrors.InvalidScopeId);
        }

        return new RuleScope(ScopeType.Country, countryId.ToString());
    }

    /// <summary>Creates a scope that targets a city.</summary>
    public static Result<RuleScope> ForCity(Guid cityId)
    {
        if (cityId == Guid.Empty)
        {
            return Result.Failure<RuleScope>(LocalRuleErrors.InvalidScopeId);
        }

        return new RuleScope(ScopeType.City, cityId.ToString());
    }

    /// <summary>Creates a scope that targets a destination.</summary>
    public static Result<RuleScope> ForDestination(Guid destinationId)
    {
        if (destinationId == Guid.Empty)
        {
            return Result.Failure<RuleScope>(LocalRuleErrors.InvalidScopeId);
        }

        return new RuleScope(ScopeType.Destination, destinationId.ToString());
    }

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Type;
        yield return TargetId;
    }
}
