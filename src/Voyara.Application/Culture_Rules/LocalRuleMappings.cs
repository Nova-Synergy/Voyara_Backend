using Voyara.Application.DTOs;
using Voyara.Domain.Entities.Culture_Rules;

namespace Voyara.Application.Culture_Rules;

/// <summary>Maps the LocalRule aggregate to its read model.</summary>
internal static class LocalRuleMappings
{
    /// <summary>Maps a local rule with its translations.</summary>
    public static LocalRuleResponse ToResponse(this LocalRule rule) => new(
        rule.Id,
        rule.Type.ToString(),
        rule.Scope.Type.ToString(),
        rule.Scope.TargetId,
        rule.IsOfficial,
        rule.Source,
        rule.LastReviewedAt,
        rule.Translations
            .Select(translation => new LocalRuleTranslationResponse(
                translation.LanguageCode,
                translation.Title,
                translation.Content))
            .ToList());
}
