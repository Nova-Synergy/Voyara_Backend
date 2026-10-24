using System.Globalization;
using Voyara.Domain.Enums;
using Voyara.SharedKernel.Results;
using Voyara.SharedKernel.ValueObjects;

namespace Voyara.Domain.Entities.Culture_Rules;

/// <summary>
/// Value object that says where a <see cref="LocalRule"/> applies: a country, a city or a destination.
/// </summary>
/// <remarks>
/// Countries and cities are identified by <c>int</c> and destinations by <c>Guid</c>. The factory methods keep
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
    public static Result<RuleScope> ForCountry(int countryId) => FromNumericId(ScopeType.Country, countryId);

    /// <summary>Creates a scope that targets a city.</summary>
    public static Result<RuleScope> ForCity(int cityId) => FromNumericId(ScopeType.City, cityId);

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

    private static Result<RuleScope> FromNumericId(ScopeType type, int id)
    {
        if (id <= 0)
        {
            return Result.Failure<RuleScope>(LocalRuleErrors.InvalidScopeId);
        }

        return new RuleScope(type, id.ToString(CultureInfo.InvariantCulture));
    }
}
