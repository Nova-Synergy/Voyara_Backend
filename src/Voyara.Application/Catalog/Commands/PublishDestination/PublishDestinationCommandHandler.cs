using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.Catalog;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Catalog.Commands.PublishDestination;

/// <summary>Handles destination publication.</summary>
public sealed class PublishDestinationCommandHandler : IRequestHandler<PublishDestinationCommand, Result>
{
    private readonly IDestinationRepository _destinationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;

    /// <summary>Creates the handler.</summary>
    public PublishDestinationCommandHandler(
        IDestinationRepository destinationRepository,
        IUnitOfWork unitOfWork,
        ICacheService cache)
    {
        _destinationRepository = destinationRepository;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(PublishDestinationCommand request, CancellationToken cancellationToken)
    {
        var destination = await _destinationRepository.GetByIdAsync(request.DestinationId, cancellationToken);
        if (destination is null)
        {
            return Result.Failure(Error.NotFound(
                "Destination.NotFound", $"Destination '{request.DestinationId}' was not found."));
        }

        destination.Publish();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // The destination appears in or disappears from public search results.
        await _cache.RemoveByTagAsync(CatalogCacheTags.Destinations, cancellationToken);

        return Result.Success();
    }
}
