using Voyara.Domain.Enums;

namespace Voyara.Api.Contracts.Culture_Rules;

/// <summary>Request body for creating a local rule with its first translation.</summary>
public sealed record CreateLocalRuleRequest(
    RuleType Type,
    ScopeType ScopeType,
    string ScopeTargetId,
    bool IsOfficial,
    string? Source,
    string LanguageCode,
    string Title,
    string Content);
