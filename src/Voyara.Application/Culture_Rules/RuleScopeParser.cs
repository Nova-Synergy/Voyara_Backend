using System.Globalization;
using Voyara.Domain.Entities.Culture_Rules;
using Voyara.Domain.Enums;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Culture_Rules;

/// <summary>Builds a <see cref="RuleScope"/> from the scope type and the target identifier received as text.</summary>
internal static class RuleScopeParser
{
    /// <summary>Parses the target identifier according to the scope type (int for country and city, Guid for destination).</summary>
    public static Result<RuleScope> Parse(ScopeType type, string? targetId)
    {
        switch (type)
        {
            case ScopeType.Country:
                return int.TryParse(targetId, NumberStyles.None, CultureInfo.InvariantCulture, out var countryId)
                    ? RuleScope.ForCountry(countryId)
                    : Result.Failure<RuleScope>(LocalRuleErrors.InvalidScopeId);

            case ScopeType.City:
                return int.TryParse(targetId, NumberStyles.None, CultureInfo.InvariantCulture, out var cityId)
                    ? RuleScope.ForCity(cityId)
                    : Result.Failure<RuleScope>(LocalRuleErrors.InvalidScopeId);

            case ScopeType.Destination:
                return Guid.TryParse(targetId, out var destinationId)
                    ? RuleScope.ForDestination(destinationId)
                    : Result.Failure<RuleScope>(LocalRuleErrors.InvalidScopeId);

            default:
                return Result.Failure<RuleScope>(LocalRuleErrors.InvalidScopeId);
        }
    }
}
