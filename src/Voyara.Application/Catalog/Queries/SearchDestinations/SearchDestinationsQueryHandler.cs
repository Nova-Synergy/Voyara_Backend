using MediatR;
using Voyara.Application.DTOs;
using Voyara.Domain.Repositories.Catalog;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Catalog.Queries.SearchDestinations;

/// <summary>Handles destination searches.</summary>
public sealed class SearchDestinationsQueryHandler
    : IRequestHandler<SearchDestinationsQuery, Result<PagedResult<DestinationSummaryResponse>>>
{
    private readonly IDestinationRepository _destinationRepository;

    /// <summary>Creates the handler.</summary>
    public SearchDestinationsQueryHandler(IDestinationRepository destinationRepository)
    {
        _destinationRepository = destinationRepository;
    }

    /// <inheritdoc />
    public async Task<Result<PagedResult<DestinationSummaryResponse>>> Handle(
        SearchDestinationsQuery request,
        CancellationToken cancellationToken)
    {
        // The validator already range-checks the paging values; the offset is still computed with a wider type
        // and re-checked so the handler can never produce a negative or overflowing offset, whoever calls it.
        var offset = (long)(request.Page - 1) * request.PageSize;
        if (request.Page < 1
            || request.Page > SearchDestinationsQueryValidator.MaxPage
            || request.PageSize < 1
            || request.PageSize > SearchDestinationsQueryValidator.MaxPageSize
            || offset > int.MaxValue)
        {
            return Result.Failure<PagedResult<DestinationSummaryResponse>>(SearchDestinationsErrors.PageOutOfRange);
        }

        var criteria = new DestinationSearchCriteria
        {
            Text = request.NormalizedSearchTerm,
            CountryId = request.CountryId,
            CityId = request.CityId,
            CategoryId = request.CategoryId,
            MinRating = request.MinRating,
            MaxPrice = request.MaxPrice,
            Currency = request.NormalizedCurrency,
            LanguageCode = request.Language,
            SortBy = request.SortBy,
            SortDirection = request.SortDirection
        };

        var destinations = await _destinationRepository.SearchAsync(
            criteria, (int)offset, request.PageSize, cancellationToken);

        // The count and the page are two queries; a catalog change between them can make the total off by a few
        // items for that one response, which is acceptable for a browse/search list.
        var totalCount = await _destinationRepository.CountAsync(criteria, cancellationToken);

        var items = destinations
            .Select(destination => destination.ToSummary(request.Language))
            .OfType<DestinationSummaryResponse>()
            .ToList();

        return new PagedResult<DestinationSummaryResponse>(items, totalCount, request.Page, request.PageSize);
    }
}
