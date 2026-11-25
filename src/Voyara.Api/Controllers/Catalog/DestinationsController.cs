using Microsoft.AspNetCore.Mvc;
using Voyara.Api.Contracts.Catalog;
using Voyara.Application.Catalog.Commands.CreateDestination;
using Voyara.Application.Catalog.Queries.GetDestinationById;
using Voyara.Application.Catalog.Queries.SearchDestinations;

namespace Voyara.Api.Controllers.Catalog;

/// <summary>Public destination endpoints (Catalog reference slice).</summary>
[Route("api/v1/[controller]")]
public sealed class DestinationsController : BaseApiController
{
    /// <summary>Searches published destinations by free text, location, category, rating and price, with sorting and paging.</summary>
    /// <param name="request">Search parameters taken from the query string.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] SearchDestinationsRequest request,
        CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new SearchDestinationsQuery(
                request.SearchTerm,
                request.Lang,
                request.CountryId,
                request.CityId,
                request.CategoryId,
                request.MinRating,
                request.MaxPrice,
                request.Currency,
                request.SortBy,
                request.SortDirection,
                request.Page,
                request.PageSize),
            cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new GetDestinationByIdQuery(id), cancellationToken));

    /// <summary>Creates a new destination in Draft status.</summary>
    /// <param name="request">Destination details, including existing city and category identifiers.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpPost]
    public async Task<IActionResult> Create(CreateDestinationRequest request, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new CreateDestinationCommand(
                request.Name,
                request.Description,
                request.Latitude,
                request.Longitude,
                request.CityId,
                request.CategoryId),
            cancellationToken));
}
