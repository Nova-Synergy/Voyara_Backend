using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyara.Api.Authorization;
using Voyara.Api.Contracts.Transportation;
using Voyara.Application.Transportation.Commands.CreateCityTransport;
using Voyara.Application.Transportation.Commands.DeleteCityTransport;
using Voyara.Application.Transportation.Commands.UpdateCityTransport;
using Voyara.Application.Transportation.Queries.GetCityTransportById;
using Voyara.Application.Transportation.Queries.GetCityTransports;

namespace Voyara.Api.Controllers.Transportation;

/// <summary>Transportation information per city (F-20): public reads, admin-only writes.</summary>
[Route("api/v1/transportations")]
public sealed class TransportationsController : BaseApiController
{
    /// <summary>Paginated, filterable list of city transports.</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? cityId,
        [FromQuery] int? transportTypeId,
        [FromQuery] Guid? destinationId,
        [FromQuery] decimal? minCost,
        [FromQuery] decimal? maxCost,
        [FromQuery] string? currency,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
        => HandleResult(await Sender.Send(
            new GetCityTransportsQuery(
                cityId, transportTypeId, destinationId, minCost, maxCost, currency, page, pageSize),
            cancellationToken));

    /// <summary>Full details of one city transport, including routes and stations.</summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new GetCityTransportByIdQuery(id), cancellationToken));

    /// <summary>Creates the transport information of a transport type in a city.</summary>
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCityTransportRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new CreateCityTransportCommand(
                request.CityId, request.TransportTypeId, request.Description, request.ApproxCost,
                request.Currency, request.OperatingHours, request.SafetyInfo),
            cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, new { id = result.Value })
            : HandleResult(result);
    }

    /// <summary>Changes the details of a city transport.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateCityTransportRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new UpdateCityTransportCommand(
                id, request.Description, request.ApproxCost, request.Currency,
                request.OperatingHours, request.SafetyInfo),
            cancellationToken);

        return result.IsSuccess ? NoContent() : HandleResult(result);
    }

    /// <summary>Soft-deletes a city transport.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new DeleteCityTransportCommand(id), cancellationToken);

        return result.IsSuccess ? NoContent() : HandleResult(result);
    }
}
