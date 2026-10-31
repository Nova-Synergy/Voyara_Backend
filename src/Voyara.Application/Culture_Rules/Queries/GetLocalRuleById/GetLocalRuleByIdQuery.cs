using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.Culture_Rules.Queries.GetLocalRuleById;

/// <summary>Gets a local rule with its translations.</summary>
public sealed record GetLocalRuleByIdQuery(Guid LocalRuleId) : IQuery<LocalRuleResponse>;
