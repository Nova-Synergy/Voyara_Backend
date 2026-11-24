using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.Catalog;
using Voyara.Domain.Repositories.Catalog;
using Voyara.Domain.Repositories.Localization;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Localization.Commands.SetDestinationTranslation;

public sealed class SetDestinationTranslationCommandHandler
    : IRequestHandler<SetDestinationTranslationCommand, Result>
{
    private readonly IDestinationRepository _destinationRepository;
    private readonly ILanguageRepository _languageRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;

    public SetDestinationTranslationCommandHandler(
        IDestinationRepository destinationRepository,
        ILanguageRepository languageRepository,
        IUnitOfWork unitOfWork,
        ICacheService cache)
    {
        _destinationRepository = destinationRepository;
        _languageRepository = languageRepository;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<Result> Handle(
        SetDestinationTranslationCommand request,
        CancellationToken cancellationToken)
    {
        var destination = await _destinationRepository.GetByIdAsync(
            request.DestinationId, cancellationToken);

        if (destination is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "Destination.NotFound",
                    "Destination was not found."));
        }

        var language = await _languageRepository.GetByCodeAsync(
            request.LanguageCode, cancellationToken);

        if (language is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "Language.NotFound",
                    "Language was not found."));
        }

        destination.SetTranslation(
            request.LanguageCode,
            request.Name,
            request.Description,
            request.CultureNotes);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Translated names and descriptions are what the search matches and displays.
        await _cache.RemoveByTagAsync(CatalogCacheTags.Destinations, cancellationToken);

        return Result.Success();
    }
}