using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.Geography;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Geography.Commands.ActivateCountry;

public sealed class ActivateCountryCommandHandler
    : IRequestHandler<ActivateCountryCommand, Result>
{
    private readonly ICountryRepository _countryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateCountryCommandHandler(
        ICountryRepository countryRepository,
        IUnitOfWork unitOfWork)
    {
        _countryRepository = countryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        ActivateCountryCommand request,
        CancellationToken cancellationToken)
    {
        var country = await _countryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (country is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "Country.NotFound",
                    $"Country with ID '{request.Id}' was not found."));
        }

        if (country.IsActive)
        {
            return Result.Failure(
                Error.Conflict(
                    "Country.AlreadyActive",
                    "The country is already active."));
        }

        country.Activate();

        _countryRepository.Update(country);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}