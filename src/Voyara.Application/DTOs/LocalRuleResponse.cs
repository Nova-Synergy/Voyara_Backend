namespace Voyara.Application.DTOs;

/// <summary>Read model of a local rule with its translations.</summary>
public sealed record LocalRuleResponse(
    Guid Id,
    string Type,
    string ScopeType,
    string ScopeTargetId,
    bool IsOfficial,
    string Source,
    DateTimeOffset LastReviewedAt,
    IReadOnlyList<LocalRuleTranslationResponse> Translations);
