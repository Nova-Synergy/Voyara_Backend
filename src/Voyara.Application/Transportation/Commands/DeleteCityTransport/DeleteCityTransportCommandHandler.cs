using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.Transportation;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Transportation.Commands.DeleteCityTransport;

/// <summary>Handles <see cref="DeleteCityTransportCommand"/>.</summary>
public sealed class DeleteCityTransportCommandHandler : IRequestHandler<DeleteCityTransportCommand, Result>
{
    private readonly ICityTransportRepository _cityTransports;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Creates the handler.</summary>
    public DeleteCityTransportCommandHandler(ICityTransportRepository cityTransports, IUnitOfWork unitOfWork)
    {
        _cityTransports = cityTransports;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(DeleteCityTransportCommand request, CancellationToken cancellationToken)
    {
        var cityTransport = await _cityTransports.GetByIdAsync(request.Id, cancellationToken);
        if (cityTransport is null)
        {
            return Result.Failure(TransportationErrors.NotFound);
        }

        _cityTransports.Remove(cityTransport);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
