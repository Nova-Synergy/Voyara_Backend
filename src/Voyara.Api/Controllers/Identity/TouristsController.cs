using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyara.Api.Authorization;
using Voyara.Application.DTOs.Identity;
using Voyara.Application.Identity.Commands.Tourists.DeleteTourist;
using Voyara.Application.Identity.Commands.Tourists.RegisterTourist;
using Voyara.Application.Identity.Commands.Tourists.UpdateTourist;
using Voyara.Application.Identity.Queries.Tourists.GetTouristById;
using Voyara.Application.Identity.Queries.Tourists.SearchTourists;

namespace Voyara.Api.Controllers.Identity;

/// <summary>Tourist accounts (user + tourist profile). Owner-or-admin rules are enforced in handlers.</summary>
[Route("api/v1/tourists")]
public sealed class TouristsController : IdentityControllerBase
{
    /// <summary>Searches tourists. Admin only.</summary>
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    public async Task<IActionResult> Search(
        [FromQuery] string? searchTerm,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
        => HandleResult(await Sender.Send(
            new SearchTouristsQuery(searchTerm, page, pageSize), cancellationToken));

    /// <summary>Gets a tourist profile. Owner or administrator.</summary>
    [HttpGet("{userId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid userId, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(new GetTouristByIdQuery(userId), cancellationToken));

    /// <summary>Registers a tourist: creates the account and profile.</summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterTouristDto dto, CancellationToken cancellationToken)
        => HandleCreated(
            await Sender.Send(
                new RegisterTouristCommand(
                    dto.FullName, dto.Email, dto.Password,
                    dto.NationalityCode, dto.PreferredLanguage, dto.Budget, dto.PhotoUrl),
                cancellationToken),
            nameof(GetById),
            t => new { userId = t.UserId });

    /// <summary>Updates a tourist's name and profile. Owner or administrator.</summary>
    [HttpPut("{userId:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid userId, UpdateTouristDto dto, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new UpdateTouristCommand(
                userId, dto.FullName, dto.NationalityCode,
                dto.PreferredLanguage, dto.Budget, dto.PhotoUrl),
            cancellationToken));

    /// <summary>Soft-deletes a tourist account. Owner or administrator.</summary>
    [HttpDelete("{userId:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid userId, CancellationToken cancellationToken)
        => HandleNoContent(await Sender.Send(new DeleteTouristCommand(userId), cancellationToken));
}
