using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.Culture_Rules;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Culture_Rules.Commands.UpdateLocalRule;

/// <summary>Handles local rule updates.</summary>
public sealed class UpdateLocalRuleCommandHandler : IRequestHandler<UpdateLocalRuleCommand, Result>
{
    private readonly ILocalRuleRepository _localRuleRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Creates the handler.</summary>
    public UpdateLocalRuleCommandHandler(ILocalRuleRepository localRuleRepository, IUnitOfWork unitOfWork)
    {
        _localRuleRepository = localRuleRepository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(UpdateLocalRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = await _localRuleRepository.GetByIdAsync(request.LocalRuleId, cancellationToken);
        if (rule is null)
        {
            return Result.Failure(CultureRulesApplicationErrors.LocalRuleNotFound(request.LocalRuleId));
        }

        var updated = rule.Update(request.Type, request.IsOfficial, request.Source);
        if (updated.IsFailure)
        {
            return Result.Failure(updated.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
