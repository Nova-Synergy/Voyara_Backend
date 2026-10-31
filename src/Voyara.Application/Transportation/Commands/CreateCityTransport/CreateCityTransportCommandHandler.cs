using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Entities.Transportation;
using Voyara.Domain.Repositories.Transportation;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Transportation.Commands.CreateCityTransport;

/// <summary>Handles city transport creation.</summary>
public sealed class CreateCityTransportCommandHandler : IRequestHandler<CreateCityTransportCommand, Result<Guid>>
{
    private readonly ICityTransportRepository _cityTransportRepository;
    private readonly ITransportTypeRepository _transportTypeRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Creates the handler.</summary>
    public CreateCityTransportCommandHandler(
        ICityTransportRepository cityTransportRepository,
        ITransportTypeRepository transportTypeRepository,
        IUnitOfWork unitOfWork)
    {
        _cityTransportRepository = cityTransportRepository;
        _transportTypeRepository = transportTypeRepository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<Guid>> Handle(CreateCityTransportCommand request, CancellationToken cancellationToken)
    {
        if (!await _transportTypeRepository.ExistsAsync(request.TransportTypeId, cancellationToken))
        {
            return Result.Failure<Guid>(TransportationApplicationErrors.TransportTypeNotFound(request.TransportTypeId));
        }

        // The (city, transport type) pair spans several aggregates, so it is checked here, not in the aggregate.
        if (await _cityTransportRepository.ExistsAsync(request.CityId, request.TransportTypeId, cancellationToken))
        {
            return Result.Failure<Guid>(TransportationApplicationErrors.CityTransportAlreadyExists);
        }

        var cityTransport = CityTransport.Create(
            request.CityId,
            request.TransportTypeId,
            request.Description,
            request.ApproxCost,
            request.Currency,
            request.OperatingHours,
            request.SafetyInfo);
        if (cityTransport.IsFailure)
        {
            return Result.Failure<Guid>(cityTransport.Error);
        }

        _cityTransportRepository.Add(cityTransport.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return cityTransport.Value.Id;
    }
}
