using Microsoft.AspNetCore.Mvc;

namespace Voyara.Api.Controllers;

/// <summary>
/// Base for controllers that call domain methods directly: an entity rejects an invalid change by throwing,
/// and that is turned into a 409 (state conflict) or a 400 (bad argument) response.
/// </summary>
[ApiController]
public abstract class DomainControllerBase : ControllerBase
{
    /// <summary>Runs a domain change; returns the failure response, or null when it succeeded.</summary>
    protected ActionResult? ApplyDomainChange(Action change)
    {
        try
        {
            change();
            return null;
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
