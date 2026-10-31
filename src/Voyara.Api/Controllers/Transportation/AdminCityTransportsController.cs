using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyara.Api.Authorization;
using Voyara.Api.Contracts.Transportation;
using Voyara.Application.Transportation.Commands.CreateCityTransport;
using Voyara.Application.Transportation.Commands.DeleteCityTransport;
using Voyara.Application.Transportation.Commands.UpdateCityTransport;

namespace Voyara.Api.Controllers.Transportation;

/// <summary>Admin endpoints that manage the transport options of cities.</summary>
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/v1/admin/[controller]")]
public sealed class AdminCityTransportsController : BaseApiController
{
    /// <summary>Creates a city transport.</summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCityTransportRequest request,
        CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new CreateCityTransportCommand(
                request.CityId,
                request.TransportTypeId,
                request.Description,
                request.ApproxCost,
                request.Currency,
                request.OperatingHours,
                request.SafetyInfo),
            cancellationToken));

    /// <summary>Updates the details of a city transport.</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateCityTransportRequest request,
        CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new UpdateCityTransportCommand(
                id,
                request.Description,
                request.ApproxCost,
                request.Currency,
                request.OperatingHours,
                request.SafetyInfo),
            cancellationToken));

    /// <summary>Deletes a city transport.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new DeleteCityTransportCommand(id), cancellationToken));
}
