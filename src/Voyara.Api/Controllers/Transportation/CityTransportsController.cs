using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyara.Application.Transportation.Queries.GetCityTransportById;
using Voyara.Application.Transportation.Queries.ListCityTransports;

namespace Voyara.Api.Controllers.Transportation;

/// <summary>Read endpoints for the transport options of cities.</summary>
[Authorize]
[Route("api/v1/[controller]")]
public sealed class CityTransportsController : BaseApiController
{
    /// <summary>Gets a city transport with its routes, stations and linked destinations.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new GetCityTransportByIdQuery(id), cancellationToken));

    /// <summary>Lists the transport options of a city.</summary>
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int cityId, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new ListCityTransportsQuery(cityId), cancellationToken));
}
