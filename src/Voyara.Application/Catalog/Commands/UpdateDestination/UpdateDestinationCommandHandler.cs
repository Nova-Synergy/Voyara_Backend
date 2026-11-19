using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Catalog;
using Voyara.Domain.ValueObjects;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Catalog.Commands.UpdateDestination;

public sealed class UpdateDestinationCommandHandler
    : IRequestHandler<UpdateDestinationCommand, Result>
{
    private readonly IDestinationRepository _destinationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDestinationCommandHandler(
        IDestinationRepository destinationRepository,
        IUnitOfWork unitOfWork)
    {
        _destinationRepository = destinationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdateDestinationCommand request,
        CancellationToken cancellationToken)
    {
        var destination = await _destinationRepository.GetByIdAsync(request.Id, cancellationToken);

        if (destination is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "Destination.NotFound",
                    $"Destination with ID '{request.Id}' was not found."));
        }

        if (destination.Status == DestinationStatus.Archived)
        {
            return Result.Failure(
                Error.Conflict(
                    "Destination.Archived",
                    "An archived destination cannot be modified."));
        }

        var location = GeoPoint.Create(request.Latitude, request.Longitude);
        if (location.IsFailure)
            return Result.Failure(location.Error);

        destination.Relocate(location.Value);
        destination.MoveToCity(request.CityId);
        destination.ChangeCategory(request.CategoryId);
        destination.ChangeEstimatedVisitTime(request.EstimatedVisitMinutes);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}