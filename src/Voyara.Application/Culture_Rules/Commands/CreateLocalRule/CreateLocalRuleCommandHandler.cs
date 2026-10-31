using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Entities.Culture_Rules;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Catalog;
using Voyara.Domain.Repositories.Culture_Rules;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Culture_Rules.Commands.CreateLocalRule;

/// <summary>Handles local rule creation.</summary>
public sealed class CreateLocalRuleCommandHandler : IRequestHandler<CreateLocalRuleCommand, Result<Guid>>
{
    private readonly ILocalRuleRepository _localRuleRepository;
    private readonly IDestinationRepository _destinationRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Creates the handler.</summary>
    public CreateLocalRuleCommandHandler(
        ILocalRuleRepository localRuleRepository,
        IDestinationRepository destinationRepository,
        IUnitOfWork unitOfWork)
    {
        _localRuleRepository = localRuleRepository;
        _destinationRepository = destinationRepository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<Guid>> Handle(CreateLocalRuleCommand request, CancellationToken cancellationToken)
    {
        var scope = RuleScopeParser.Parse(request.ScopeType, request.ScopeTargetId);
        if (scope.IsFailure)
        {
            return Result.Failure<Guid>(scope.Error);
        }

        // Destinations belong to the Catalog module, so their existence is checked here. Countries and cities
        // have no repository yet and are referenced by Id only.
        if (request.ScopeType == ScopeType.Destination)
        {
            var destinationId = Guid.Parse(scope.Value.TargetId);
            var destination = await _destinationRepository.GetByIdAsync(destinationId, cancellationToken);
            if (destination is null)
            {
                return Result.Failure<Guid>(CultureRulesApplicationErrors.DestinationNotFound(destinationId));
            }
        }

        var translation = LocalRuleTranslation.Create(request.LanguageCode, request.Title, request.Content);
        if (translation.IsFailure)
        {
            return Result.Failure<Guid>(translation.Error);
        }

        var rule = LocalRule.Create(request.Type, scope.Value, request.IsOfficial, request.Source, translation.Value);
        if (rule.IsFailure)
        {
            return Result.Failure<Guid>(rule.Error);
        }

        _localRuleRepository.Add(rule.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return rule.Value.Id;
    }
}
