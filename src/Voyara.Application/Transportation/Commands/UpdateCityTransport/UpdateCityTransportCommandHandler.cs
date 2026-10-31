using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.Transportation;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Transportation.Commands.UpdateCityTransport;

/// <summary>Handles city transport updates.</summary>
public sealed class UpdateCityTransportCommandHandler : IRequestHandler<UpdateCityTransportCommand, Result>
{
    private readonly ICityTransportRepository _cityTransportRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Creates the handler.</summary>
    public UpdateCityTransportCommandHandler(ICityTransportRepository cityTransportRepository, IUnitOfWork unitOfWork)
    {
        _cityTransportRepository = cityTransportRepository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(UpdateCityTransportCommand request, CancellationToken cancellationToken)
    {
        var cityTransport = await _cityTransportRepository.GetByIdAsync(request.CityTransportId, cancellationToken);
        if (cityTransport is null)
        {
            return Result.Failure(TransportationApplicationErrors.CityTransportNotFound(request.CityTransportId));
        }

        var updated = cityTransport.UpdateDetails(
            request.Description,
            request.ApproxCost,
            request.Currency,
            request.OperatingHours,
            request.SafetyInfo);
        if (updated.IsFailure)
        {
            return Result.Failure(updated.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
