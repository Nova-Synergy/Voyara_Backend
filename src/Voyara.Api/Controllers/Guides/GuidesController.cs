using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyara.Api.Contracts.Guides;
using Voyara.Application.Guides.Commands.CreateGuideProfile;
using Voyara.Application.Guides.Commands.DeleteGuideDocument;
using Voyara.Application.Guides.Commands.SubmitGuideApplication;
using Voyara.Application.Guides.Commands.UpdateGuideProfile;
using Voyara.Application.Guides.Commands.UploadGuideDocument;
using Voyara.Application.Guides.Queries.GetApprovedGuides;
using Voyara.Application.Guides.Queries.GetGuideDocument;
using Voyara.Application.Guides.Queries.GetGuideProfileDetails;
using Voyara.Application.Guides.Queries.GetMyGuideProfile;
using Voyara.Domain.Enums;
using Voyara.SharedKernel.Results;

namespace Voyara.Api.Controllers.Guides;

/// <summary>Guide directory, own-profile management and verification documents (F-18).</summary>
[ApiController]
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
    public async Task<IActionResult> Create([FromBody] CreateGuideProfileRequest request, CancellationToken cancellationToken)
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

    /// <summary>Uploads a verification document into private storage.</summary>
    [HttpPost("profile/documents")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadDocument(
        [FromForm] UploadGuideDocumentRequest request,
        CancellationToken cancellationToken)
    {
        if (request.File is null || request.File.Length == 0)
        {
            return HandleResult(Result.Failure<Guid>(
                Error.Validation("Guides.InvalidDocument", "A non-empty file is required.")));
        }

        await using var stream = request.File.OpenReadStream();
        var result = await Sender.Send(
            new UploadGuideDocumentCommand(
                request.DocumentType, request.File.FileName, request.File.ContentType, request.File.Length, stream),
            cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(DownloadDocument), new { documentId = result.Value }, new { id = result.Value })
            : HandleResult(result);
    }

    /// <summary>Downloads a verification document (owner or admin only).</summary>
    [HttpGet("documents/{documentId:guid}")]
    [Authorize]
    public async Task<IActionResult> DownloadDocument(Guid documentId, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetGuideDocumentQuery(documentId), cancellationToken);

        return result.IsSuccess
            ? File(result.Value.Content, result.Value.ContentType, result.Value.FileName)
            : HandleResult(result);
    }

    /// <summary>Deletes one of the caller's own verification documents.</summary>
    [HttpDelete("documents/{documentId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteDocument(Guid documentId, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new DeleteGuideDocumentCommand(documentId), cancellationToken));
}
