using Voyara.Application.Abstractions.Messaging;
using Voyara.Domain.Enums;

namespace Voyara.Application.Culture_Rules.Commands.UpdateLocalRule;

/// <summary>Changes the category, the official flag and the source of a rule. The scope never changes.</summary>
public sealed record UpdateLocalRuleCommand(Guid LocalRuleId, RuleType Type, bool IsOfficial, string? Source) : ICommand;
