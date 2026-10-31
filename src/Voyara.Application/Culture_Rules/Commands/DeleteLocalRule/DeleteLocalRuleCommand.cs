using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Culture_Rules.Commands.DeleteLocalRule;

/// <summary>Deletes a local rule (soft delete).</summary>
public sealed record DeleteLocalRuleCommand(Guid LocalRuleId) : ICommand;
