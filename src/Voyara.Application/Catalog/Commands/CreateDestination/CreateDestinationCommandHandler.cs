using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Repositories.Catalog;
using Voyara.Domain.ValueObjects;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Catalog.Commands.CreateDestination;

public sealed class CreateDestinationCommandHandler
    : IRequestHandler<CreateDestinationCommand, Result<Guid>>
{
    private readonly IDestinationRepository _destinationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDestinationCommandHandler(
        IDestinationRepository destinationRepository,
        IUnitOfWork unitOfWork)
    {
        _destinationRepository = destinationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreateDestinationCommand request,
        CancellationToken cancellationToken)
    {
        var location = GeoPoint.Create(request.Latitude, request.Longitude);
        if (location.IsFailure)
            return Result.Failure<Guid>(location.Error);

        var destination = Destination.Create(
            location.Value,
            request.CityId,
            request.CategoryId,
            request.EstimatedVisitMinutes,
            request.LanguageCode,
            request.Name,
            request.Description,
            request.CultureNotes);

        _destinationRepository.Add(destination);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return destination.Id;
    }
}