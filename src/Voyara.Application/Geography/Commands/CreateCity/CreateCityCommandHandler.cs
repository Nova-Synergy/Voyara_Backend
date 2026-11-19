using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.Geography;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Geography.Commands.CreateCity;

public sealed class CreateCityCommandHandler
    : IRequestHandler<CreateCityCommand, Result<int>>
{
    private readonly ICountryRepository _countryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCityCommandHandler(
        ICountryRepository countryRepository,
        IUnitOfWork unitOfWork)
    {
        _countryRepository = countryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(
        CreateCityCommand request,
        CancellationToken cancellationToken)
    {
        var country = await _countryRepository.GetByIdAsync(request.CountryId, cancellationToken);

        if (country is null)
        {
            return Result.Failure<int>(
                Error.NotFound(
                    "Country.NotFound",
                    $"Country with ID '{request.CountryId}' was not found."));
        }

        var city = country.AddCity(request.Latitude, request.Longitude);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return city.Id;
    }
}