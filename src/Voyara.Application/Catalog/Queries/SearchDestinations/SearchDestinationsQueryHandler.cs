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
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var skip = (page - 1) * pageSize;

        var criteria = new DestinationSearchCriteria
        {
            Text = request.SearchTerm,
            LanguageCode = request.Language
        };

        var destinations = await _destinationRepository.SearchAsync(criteria, skip, pageSize, cancellationToken);
        var totalCount = await _destinationRepository.CountAsync(criteria, cancellationToken);

        var items = destinations
            .Select(destination => destination.ToSummary(request.Language))
            .ToList();

        return new PagedResult<DestinationSummaryResponse>(items, totalCount, page, pageSize);
    }
}
