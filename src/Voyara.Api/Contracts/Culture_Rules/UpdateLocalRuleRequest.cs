using Voyara.Domain.Enums;

namespace Voyara.Api.Contracts.Culture_Rules;

/// <summary>Request body for updating the category, the official flag and the source of a local rule.</summary>
public sealed record UpdateLocalRuleRequest(RuleType Type, bool IsOfficial, string? Source);
