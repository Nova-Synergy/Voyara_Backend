using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyara.Api.Contracts.Guide;
using Voyara.Application.Guides.Commands.CreateGuideProfile;
using Voyara.Application.Guides.Commands.SubmitGuideApplication;
using Voyara.Application.Guides.Commands.UpdateGuideProfile;
using Voyara.Application.Guides.Queries.GetApprovedGuides;
using Voyara.Application.Guides.Queries.GetGuideProfileDetails;
using Voyara.Application.Guides.Queries.GetMyGuideProfile;
using Voyara.Domain.Enums;

namespace Voyara.Api.Controllers.Guide;

/// <summary>Guide directory and own-profile management (F-18).</summary>
[Route("api/v1/guides")]
public sealed class GuidesController : BaseApiController
{
    /// <summary>Public directory of approved guides with filters and paging.</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetApproved(
        [FromQuery] Guid? cityId,
        [FromQuery] string? language,
        [FromQuery] int? expertiseCategoryId,
        [FromQuery] decimal? minRating,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] PricingUnit? pricingUnit,
        [FromQuery] string? currency,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
        => HandleResult(await Sender.Send(
            new GetApprovedGuidesQuery(
                cityId, language, expertiseCategoryId, minRating, minPrice, maxPrice,
                pricingUnit, currency, page, pageSize),
            cancellationToken));

    /// <summary>Guide profile details. Non-approved profiles are visible only to the owner and admins.</summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new GetGuideProfileDetailsQuery(id), cancellationToken));

    /// <summary>The authenticated user's own profile, in any status.</summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new GetMyGuideProfileQuery(), cancellationToken));

    /// <summary>Creates the authenticated user's guide profile (status Pending).</summary>
    [HttpPost("profile")]
    [Authorize]
    public async Task<IActionResult> Create(
        [FromBody] CreateGuideProfileRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new CreateGuideProfileCommand(
                request.CityId, request.Bio, request.PhotoUrl, request.YearsOfExperience,
                request.Languages, request.ExpertiseCategoryIds, request.Availability, request.Pricing),
            cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, new { id = result.Value })
            : HandleResult(result);
    }

    /// <summary>Updates the authenticated user's own profile.</summary>
    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> Update(
        [FromBody] UpdateGuideProfileRequest request,
        CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new UpdateGuideProfileCommand(
                request.Bio, request.PhotoUrl, request.YearsOfExperience,
                request.Languages, request.ExpertiseCategoryIds, request.Availability, request.Pricing),
            cancellationToken));

    /// <summary>Checks that the pending application is complete and ready for admin review.</summary>
    [HttpPost("profile/submit")]
    [Authorize]
    public async Task<IActionResult> Submit(CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new SubmitGuideApplicationCommand(), cancellationToken));
}
