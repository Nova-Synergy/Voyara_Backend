using MediatR;
using Voyara.Domain.Entities.Localization;
using Voyara.Domain.Repositories.Catalog;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Localization.Queries.GetDestinationTranslations;

public sealed class GetDestinationTranslationsQueryHandler
    : IRequestHandler<GetDestinationTranslationsQuery,
        Result<IReadOnlyCollection<DestinationTranslation>>>
{
    private readonly IDestinationRepository _repository;

    public GetDestinationTranslationsQueryHandler(
        IDestinationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyCollection<DestinationTranslation>>> Handle(
        GetDestinationTranslationsQuery request,
        CancellationToken cancellationToken)
    {
        var destination = await _repository.GetByIdAsync(
            request.DestinationId, cancellationToken);

        if (destination is null)
        {
            return Result.Failure<IReadOnlyCollection<DestinationTranslation>>(
                Error.NotFound(
                    "Destination.NotFound",
                    "Destination was not found."));
        }

        return destination.Translations;
    }
}