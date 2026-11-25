using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Repositories.Catalog;
using Voyara.Domain.Repositories.Geography;
using Voyara.Domain.ValueObjects;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Catalog.Commands.CreateDestination;

/// <summary>Handles destination creation after validating its required relationships.</summary>
public sealed class CreateDestinationCommandHandler : IRequestHandler<CreateDestinationCommand, Result<Guid>>
{
    private readonly IDestinationRepository _destinationRepository;
    private readonly ICityRepository _cityRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initializes a new instance of the <see cref="CreateDestinationCommandHandler"/> class.</summary>
    /// <param name="destinationRepository">Repository used to add the destination.</param>
    /// <param name="cityRepository">Repository used to verify the selected city.</param>
    /// <param name="categoryRepository">Repository used to verify the selected category.</param>
    /// <param name="unitOfWork">Unit of work used to persist the destination.</param>
    public CreateDestinationCommandHandler(
        IDestinationRepository destinationRepository,
        ICityRepository cityRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _destinationRepository = destinationRepository;
        _cityRepository = cityRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<Guid>> Handle(CreateDestinationCommand request, CancellationToken cancellationToken)
    {
        var location = GeoPoint.Create(request.Latitude, request.Longitude);
        if (location.IsFailure)
        {
            return Result.Failure<Guid>(location.Error);
        }

        var city = await _cityRepository.GetByIdAsync(request.CityId, cancellationToken);
        if (city is null)
        {
            return Result.Failure<Guid>(Error.NotFound(
                "City.NotFound",
                $"City '{request.CityId}' was not found."));
        }

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null)
        {
            return Result.Failure<Guid>(Error.NotFound(
                "Category.NotFound",
                $"Category '{request.CategoryId}' was not found."));
        }

        var destination = Destination.Create(
            request.Name,
            request.Description,
            location.Value,
            request.CityId,
            request.CategoryId);

        _destinationRepository.Add(destination);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return destination.Id;
    }
}
