using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Entities.Transportation;
using Voyara.Domain.Repositories.Transportation;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Transportation.Commands.CreateCityTransport;

/// <summary>Handles <see cref="CreateCityTransportCommand"/>.</summary>
public sealed class CreateCityTransportCommandHandler
    : IRequestHandler<CreateCityTransportCommand, Result<Guid>>
{
    private readonly ICityTransportRepository _cityTransports;
    private readonly ITransportTypeRepository _transportTypes;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Creates the handler.</summary>
    public CreateCityTransportCommandHandler(
        ICityTransportRepository cityTransports,
        ITransportTypeRepository transportTypes,
        IUnitOfWork unitOfWork)
    {
        _cityTransports = cityTransports;
        _transportTypes = transportTypes;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<Guid>> Handle(CreateCityTransportCommand request, CancellationToken cancellationToken)
    {
        var created = CityTransport.Create(
            request.CityId,
            request.TransportTypeId,
            request.Description,
            request.ApproxCost,
            request.Currency,
            request.OperatingHours,
            request.SafetyInfo);

        if (created.IsFailure)
        {
            return Result.Failure<Guid>(created.Error);
        }

        if (!await _transportTypes.ExistsAsync(request.TransportTypeId, cancellationToken))
        {
            return Result.Failure<Guid>(TransportationErrors.TransportTypeNotFound);
        }

        if (await _cityTransports.ExistsAsync(request.CityId, request.TransportTypeId, cancellationToken))
        {
            return Result.Failure<Guid>(TransportationErrors.AlreadyExists);
        }

        _cityTransports.Add(created.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return created.Value.Id;
    }
}
