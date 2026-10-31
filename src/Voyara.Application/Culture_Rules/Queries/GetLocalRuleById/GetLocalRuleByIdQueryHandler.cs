using MediatR;
using Voyara.Application.DTOs;
using Voyara.Domain.Repositories.Culture_Rules;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Culture_Rules.Queries.GetLocalRuleById;

/// <summary>Handles single local rule lookups.</summary>
public sealed class GetLocalRuleByIdQueryHandler : IRequestHandler<GetLocalRuleByIdQuery, Result<LocalRuleResponse>>
{
    private readonly ILocalRuleRepository _localRuleRepository;

    /// <summary>Creates the handler.</summary>
    public GetLocalRuleByIdQueryHandler(ILocalRuleRepository localRuleRepository)
        => _localRuleRepository = localRuleRepository;

    /// <inheritdoc />
    public async Task<Result<LocalRuleResponse>> Handle(GetLocalRuleByIdQuery request, CancellationToken cancellationToken)
    {
        var rule = await _localRuleRepository.GetByIdAsync(request.LocalRuleId, cancellationToken);
        if (rule is null)
        {
            return Result.Failure<LocalRuleResponse>(CultureRulesApplicationErrors.LocalRuleNotFound(request.LocalRuleId));
        }

        return rule.ToResponse();
    }
}
