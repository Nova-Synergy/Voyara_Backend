using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.Transportation;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Transportation.Commands.UpdateCityTransport;

/// <summary>Handles <see cref="UpdateCityTransportCommand"/>.</summary>
public sealed class UpdateCityTransportCommandHandler : IRequestHandler<UpdateCityTransportCommand, Result>
{
    private readonly ICityTransportRepository _cityTransports;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Creates the handler.</summary>
    public UpdateCityTransportCommandHandler(ICityTransportRepository cityTransports, IUnitOfWork unitOfWork)
    {
        _cityTransports = cityTransports;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(UpdateCityTransportCommand request, CancellationToken cancellationToken)
    {
        var cityTransport = await _cityTransports.GetByIdAsync(request.Id, cancellationToken);
        if (cityTransport is null)
        {
            return Result.Failure(TransportationErrors.NotFound);
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
