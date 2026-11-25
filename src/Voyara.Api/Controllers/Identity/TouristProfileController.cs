using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyara.Api.Authorization;
using Voyara.Api.Contracts.Identity;
using Voyara.Application.Identity.Commands.RemoveMyProfileImage;
using Voyara.Application.Identity.Commands.UpdateMyPreferences;
using Voyara.Application.Identity.Commands.UpdateMyTouristProfile;
using Voyara.Application.Identity.Commands.UploadMyProfileImage;
using Voyara.Application.Identity.Queries.GetAvailableInterests;
using Voyara.Application.Identity.Queries.GetMyProfileImage;
using Voyara.Application.Identity.Queries.GetMyTouristProfile;

namespace Voyara.Api.Controllers.Identity;

/// <summary>Profile, preferences and profile image endpoints of the signed-in tourist (F02).</summary>
[Authorize(Policy = AuthorizationPolicies.TouristOnly)]
[Route("api/v1/profile")]
public sealed class TouristProfileController : BaseApiController
{
    private const long MaxUploadRequestBytes = 6 * 1024 * 1024;

    /// <summary>Gets the profile and preferences of the current tourist.</summary>
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new GetMyTouristProfileQuery(), cancellationToken));

    /// <summary>Updates the display name and nationality of the current tourist.</summary>
    [HttpPut]
    public async Task<IActionResult> Update(UpdateTouristProfileRequest request, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new UpdateMyTouristProfileCommand(request.FullName, request.NationalityCode),
            cancellationToken));

    /// <summary>Lists the interests the tourist can choose from.</summary>
    [HttpGet("interests")]
    public async Task<IActionResult> GetInterests(CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new GetAvailableInterestsQuery(), cancellationToken));

    /// <summary>Replaces the preferred language, budget and interests of the current tourist.</summary>
    [HttpPut("preferences")]
    public async Task<IActionResult> UpdatePreferences(
        UpdateTouristPreferencesRequest request,
        CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new UpdateMyPreferencesCommand(request.PreferredLanguage, request.Budget, request.InterestCodes ?? Array.Empty<string>()),
            cancellationToken));

    /// <summary>Streams the stored profile image of the current tourist.</summary>
    [HttpGet("photo")]
    public async Task<IActionResult> GetPhoto(CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetMyProfileImageQuery(), cancellationToken);
        if (result.IsFailure)
        {
            return HandleResult(result);
        }

        return File(result.Value.Content, result.Value.ContentType);
    }

    /// <summary>Sets or replaces the profile image (JPEG, PNG or WebP, up to 5 MB).</summary>
    [HttpPut("photo")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(MaxUploadRequestBytes)]
    public async Task<IActionResult> UploadPhoto(IFormFile file, CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();

        return HandleResult(await Sender.Send(
            new UploadMyProfileImageCommand(stream, file.ContentType, file.Length),
            cancellationToken));
    }

    /// <summary>Removes the profile image of the current tourist.</summary>
    [HttpDelete("photo")]
    public async Task<IActionResult> DeletePhoto(CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new RemoveMyProfileImageCommand(), cancellationToken));
}
