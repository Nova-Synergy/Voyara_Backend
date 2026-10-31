using MediatR;
using Voyara.Application.DTOs;
using Voyara.Domain.Repositories.Culture_Rules;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Culture_Rules.Queries.ListLocalRulesByScope;

/// <summary>Handles local rule lists by scope.</summary>
public sealed class ListLocalRulesByScopeQueryHandler
    : IRequestHandler<ListLocalRulesByScopeQuery, Result<List<LocalRuleResponse>>>
{
    private readonly ILocalRuleRepository _localRuleRepository;

    /// <summary>Creates the handler.</summary>
    public ListLocalRulesByScopeQueryHandler(ILocalRuleRepository localRuleRepository)
        => _localRuleRepository = localRuleRepository;

    /// <inheritdoc />
    public async Task<Result<List<LocalRuleResponse>>> Handle(
        ListLocalRulesByScopeQuery request,
        CancellationToken cancellationToken)
    {
        var scope = RuleScopeParser.Parse(request.ScopeType, request.ScopeTargetId);
        if (scope.IsFailure)
        {
            return Result.Failure<List<LocalRuleResponse>>(scope.Error);
        }

        var rules = await _localRuleRepository.ListByScopesAsync(new[] { scope.Value }, cancellationToken);

        var items = rules
            .Select(rule => rule.ToResponse())
            .ToList();

        return items;
    }
}
