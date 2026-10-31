using Voyara.Application.Abstractions.Messaging;
using Voyara.Domain.Enums;

namespace Voyara.Application.Culture_Rules.Commands.CreateLocalRule;

/// <summary>
/// Creates a local rule with its first translation; returns the rule identifier.
/// <paramref name="ScopeTargetId"/> is an int for countries and cities, and a Guid for destinations.
/// </summary>
public sealed record CreateLocalRuleCommand(
    RuleType Type,
    ScopeType ScopeType,
    string ScopeTargetId,
    bool IsOfficial,
    string? Source,
    string LanguageCode,
    string Title,
    string Content) : ICommand<Guid>;
