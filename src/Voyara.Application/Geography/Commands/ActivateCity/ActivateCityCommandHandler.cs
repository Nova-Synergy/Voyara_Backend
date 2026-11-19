using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.Geography;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Geography.Commands.ActivateCity;

public sealed class ActivateCityCommandHandler
    : IRequestHandler<ActivateCityCommand, Result>
{
    private readonly ICountryRepository _countryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateCityCommandHandler(
        ICountryRepository countryRepository,
        IUnitOfWork unitOfWork)
    {
        _countryRepository = countryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        ActivateCityCommand request,
        CancellationToken cancellationToken)
    {
        var country = await _countryRepository.GetByCityIdAsync(request.Id, cancellationToken);
        var city = country?.Cities.FirstOrDefault(c => c.Id == request.Id);

        if (city is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "City.NotFound",
                    $"City with ID '{request.Id}' was not found."));
        }

        if (city.IsActive)
        {
            return Result.Failure(
                Error.Conflict(
                    "City.AlreadyActive",
                    "The city is already active."));
        }

        city.Activate();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}