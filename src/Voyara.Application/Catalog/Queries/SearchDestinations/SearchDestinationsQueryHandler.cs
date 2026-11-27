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
        // The validator already range-checks the paging values; the handler repeats the check before doing any
        // arithmetic, so whoever calls it (the pipeline, another handler, a test) can never reach an overflow.
        if (request.Page < 1
            || request.Page > SearchDestinationsQueryValidator.MaxPage
            || request.PageSize < 1
            || request.PageSize > SearchDestinationsQueryValidator.MaxPageSize)
        {
            return Result.Failure<PagedResult<DestinationSummaryResponse>>(SearchDestinationsErrors.PageOutOfRange);
        }

        // Widen before subtracting; after the range check above the offset is at most (MaxPage - 1) * MaxPageSize.
        var offset = ((long)request.Page - 1) * request.PageSize;
        if (offset > int.MaxValue)
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

        var searchPage = await _destinationRepository.SearchPageAsync(
            criteria, (int)offset, request.PageSize, cancellationToken);

        var items = searchPage.Items
            .Select(destination => destination.ToSummary(request.Language))
            .OfType<DestinationSummaryResponse>()
            .ToList();

        return new PagedResult<DestinationSummaryResponse>(
            items, searchPage.TotalCount, request.Page, request.PageSize);
    }
}
