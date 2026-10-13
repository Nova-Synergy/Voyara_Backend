using MapsterMapper;
using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs;
using Voyara.Domain.Repositories.Catalog;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Catalog.Queries.SearchDestinations;

/// <summary>Handles destination searches.</summary>
public sealed class SearchDestinationsQueryHandler
    : IRequestHandler<SearchDestinationsQuery, Result<PagedResult<DestinationResponse>>>
{
    private readonly IDestinationRepository _destinationRepository;
    private readonly IMapper _mapper;

    /// <summary>Creates the handler.</summary>
    public SearchDestinationsQueryHandler(IDestinationRepository destinationRepository, IMapper mapper)
    {
        _destinationRepository = destinationRepository;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<Result<PagedResult<DestinationResponse>>> Handle(
        SearchDestinationsQuery request,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var skip = (page - 1) * pageSize;

        var destinations = await _destinationRepository.SearchAsync(
            request.SearchTerm, null, skip, pageSize, cancellationToken);
        var totalCount = await _destinationRepository.CountAsync(
            request.SearchTerm, null, cancellationToken);

        var items = destinations
            .Select(destination => _mapper.Map<DestinationResponse>(destination))
            .ToList();

        return new PagedResult<DestinationResponse>(items, totalCount, page, pageSize);
    }
}
