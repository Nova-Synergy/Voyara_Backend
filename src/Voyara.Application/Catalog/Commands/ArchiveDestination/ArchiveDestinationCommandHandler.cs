using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.Catalog;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Catalog.Commands.ArchiveDestination;

/// <summary>Handles destination archiving.</summary>
public sealed class ArchiveDestinationCommandHandler : IRequestHandler<ArchiveDestinationCommand, Result>
{
    private readonly IDestinationRepository _destinationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;

    /// <summary>Creates the handler.</summary>
    public ArchiveDestinationCommandHandler(
        IDestinationRepository destinationRepository,
        IUnitOfWork unitOfWork,
        ICacheService cache)
    {
        _destinationRepository = destinationRepository;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(ArchiveDestinationCommand request, CancellationToken cancellationToken)
    {
        var destination = await _destinationRepository.GetByIdAsync(request.DestinationId, cancellationToken);
        if (destination is null)
        {
            return Result.Failure(Error.NotFound(
                "Destination.NotFound", $"Destination '{request.DestinationId}' was not found."));
        }

        destination.Archive();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // The destination appears in or disappears from public search results.
        await _cache.RemoveByTagAsync(CatalogCacheTags.Destinations, cancellationToken);

        return Result.Success();
    }
}
