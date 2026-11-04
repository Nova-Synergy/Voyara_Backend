using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyara.Application.DTOs.Identity;
using Voyara.Application.Identity.Commands.TourGuides.DeleteTourGuide;
using Voyara.Application.Identity.Commands.TourGuides.RegisterTourGuide;
using Voyara.Application.Identity.Commands.TourGuides.UpdateTourGuide;
using Voyara.Application.Identity.Queries.TourGuides.GetTourGuideById;
using Voyara.Application.Identity.Queries.TourGuides.SearchTourGuides;
using Voyara.Domain.Enums;

namespace Voyara.Api.Controllers.Identity;

/// <summary>
/// Tour guide accounts. Anyone can apply (profile starts Pending); visibility and owner-or-admin rules
/// are enforced in handlers.
/// </summary>
[Route("api/v1/tour-guides")]
public sealed class TourGuidesController : IdentityControllerBase
{
    /// <summary>Lists guides with paging. The public sees approved guides; an administrator can filter by status.</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Search(
        [FromQuery] int? cityId, [FromQuery] GuideStatus? status,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
        => HandleResult(await Sender.Send(
            new SearchTourGuidesQuery(cityId, status, page, pageSize), cancellationToken));

    /// <summary>Gets a guide profile. Non-approved profiles are visible only to the owner and administrators.</summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new GetTourGuideByIdQuery(id), cancellationToken));

    /// <summary>Registers a tour guide: creates the account (TourGuide role) and a Pending profile.</summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterTourGuideDto dto, CancellationToken cancellationToken)
        => HandleCreated(
            await Sender.Send(
                new RegisterTourGuideCommand(
                    dto.FullName, dto.Email, dto.Password,
                    dto.CityId, dto.Bio, dto.PhotoUrl, dto.YearsOfExperience),
                cancellationToken),
            nameof(GetById),
            g => new { id = g.Id });

    /// <summary>Updates a guide's name and profile details. The owner or an administrator.</summary>
    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, UpdateTourGuideDto dto, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new UpdateTourGuideCommand(id, dto.FullName, dto.Bio, dto.PhotoUrl, dto.YearsOfExperience),
            cancellationToken));

    /// <summary>Soft-deletes the guide profile (the user account stays). The owner or an administrator.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => HandleNoContent(await Sender.Send(new DeleteTourGuideCommand(id), cancellationToken));
}
