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

    /// <summary>Creates the handler.</summary>
    public PublishDestinationCommandHandler(IDestinationRepository destinationRepository, IUnitOfWork unitOfWork)
    {
        _destinationRepository = destinationRepository;
        _unitOfWork = unitOfWork;
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

        return Result.Success();
    }
}
