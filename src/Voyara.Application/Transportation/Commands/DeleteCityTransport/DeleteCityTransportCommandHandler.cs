using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.Transportation;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Transportation.Commands.DeleteCityTransport;

/// <summary>Handles city transport deletion.</summary>
public sealed class DeleteCityTransportCommandHandler : IRequestHandler<DeleteCityTransportCommand, Result>
{
    private readonly ICityTransportRepository _cityTransportRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Creates the handler.</summary>
    public DeleteCityTransportCommandHandler(ICityTransportRepository cityTransportRepository, IUnitOfWork unitOfWork)
    {
        _cityTransportRepository = cityTransportRepository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(DeleteCityTransportCommand request, CancellationToken cancellationToken)
    {
        var cityTransport = await _cityTransportRepository.GetByIdAsync(request.CityTransportId, cancellationToken);
        if (cityTransport is null)
        {
            return Result.Failure(TransportationApplicationErrors.CityTransportNotFound(request.CityTransportId));
        }

        cityTransport.Delete();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
