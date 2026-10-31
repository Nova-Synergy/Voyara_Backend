using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;
using Voyara.Domain.Enums;

namespace Voyara.Application.Culture_Rules.Queries.ListLocalRulesByScope;

/// <summary>
/// Lists the rules of one scope (a country, a city or a destination).
/// <paramref name="ScopeTargetId"/> is an int for countries and cities, and a Guid for destinations.
/// </summary>
public sealed record ListLocalRulesByScopeQuery(ScopeType ScopeType, string ScopeTargetId)
    : IQuery<List<LocalRuleResponse>>;
