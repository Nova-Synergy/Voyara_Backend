using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.Culture_Rules;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Culture_Rules.Commands.DeleteLocalRule;

/// <summary>Handles local rule deletion.</summary>
public sealed class DeleteLocalRuleCommandHandler : IRequestHandler<DeleteLocalRuleCommand, Result>
{
    private readonly ILocalRuleRepository _localRuleRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Creates the handler.</summary>
    public DeleteLocalRuleCommandHandler(ILocalRuleRepository localRuleRepository, IUnitOfWork unitOfWork)
    {
        _localRuleRepository = localRuleRepository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(DeleteLocalRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = await _localRuleRepository.GetByIdAsync(request.LocalRuleId, cancellationToken);
        if (rule is null)
        {
            return Result.Failure(CultureRulesApplicationErrors.LocalRuleNotFound(request.LocalRuleId));
        }

        rule.Delete();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
