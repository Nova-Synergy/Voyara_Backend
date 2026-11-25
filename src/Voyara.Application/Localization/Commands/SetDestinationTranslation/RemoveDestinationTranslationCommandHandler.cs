using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.Catalog;
using Voyara.Domain.Repositories.Catalog;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Localization.Commands.RemoveDestinationTranslation;

/// <summary>Handles removal of a destination translation.</summary>
public sealed class RemoveDestinationTranslationCommandHandler
    : IRequestHandler<RemoveDestinationTranslationCommand, Result>
{
    private readonly IDestinationRepository _destinationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;

    /// <summary>Initializes a new instance of the <see cref="RemoveDestinationTranslationCommandHandler"/> class.</summary>
    /// <param name="destinationRepository">Repository used to load the destination.</param>
    /// <param name="unitOfWork">Unit of work used to persist the change.</param>
    /// <param name="cache">Cache service used to invalidate destination search results.</param>
    public RemoveDestinationTranslationCommandHandler(
        IDestinationRepository destinationRepository,
        IUnitOfWork unitOfWork,
        ICacheService cache)
    {
        _destinationRepository = destinationRepository;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(
        RemoveDestinationTranslationCommand request,
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

        destination.RemoveTranslation(request.LanguageCode);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cache.RemoveByTagAsync(CatalogCacheTags.Destinations, cancellationToken);

        return Result.Success();
    }
}
