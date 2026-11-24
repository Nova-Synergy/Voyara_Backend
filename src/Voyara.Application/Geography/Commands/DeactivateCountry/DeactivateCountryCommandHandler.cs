using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.Geography;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Geography.Commands.DeactivateCountry;

public sealed class DeactivateCountryCommandHandler
    : IRequestHandler<DeactivateCountryCommand, Result>
{
    private readonly ICountryRepository _countryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateCountryCommandHandler(
        ICountryRepository countryRepository,
        IUnitOfWork unitOfWork)
    {
        _countryRepository = countryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeactivateCountryCommand request,
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

        if (!country.IsActive)
        {
            return Result.Failure(
                Error.Conflict(
                    "Country.AlreadyInactive",
                    "The country is already inactive."));
        }

        country.Deactivate();

        _countryRepository.Update(country);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}