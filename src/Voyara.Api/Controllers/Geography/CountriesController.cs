using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Voyara.Api.Authorization;
using Voyara.Application.DTOs.Geography;
using Voyara.Application.Abstractions;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Domain.Entities.Geography;
using Voyara.Infrastructure.Persistence;

namespace Voyara.Api.Controllers.Geography;

/// <summary>
/// Countries. Reading is public (tourists see active countries only; administrators can include inactive ones);
/// creating, updating and deleting is for administrators.
/// </summary>
[Route("api/v1/countries")]
public class CountriesController : DomainControllerBase
{
    private readonly VoyaraDbContext _context;
    private readonly ICurrentUser _currentUser;

    public CountriesController(VoyaraDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    private bool IsAdmin => _currentUser.IsInRole(Roles.Admin);

    /// <summary>Lists countries ordered by ISO code.</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<CountryReadDto>>> GetAll(
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Countries.AsNoTracking();
        if (!(includeInactive && IsAdmin))
        {
            query = query.Where(c => c.IsActive);
        }

        var countries = await query.OrderBy(c => c.IsoCode).ToListAsync(cancellationToken);

        return Ok(countries.Select(CountryReadDto.FromEntity).ToList());
    }

    /// <summary>Gets a country. Inactive countries are visible to administrators only.</summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<CountryReadDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var country = await _context.Countries.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (country is null || (!country.IsActive && !IsAdmin))
        {
            return NotFound();
        }

        return Ok(CountryReadDto.FromEntity(country));
    }

    /// <summary>Creates an active country. Admin only.</summary>
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    public async Task<ActionResult<CountryReadDto>> Create(CreateCountryDto dto, CancellationToken cancellationToken)
    {
        var isoCode = dto.IsoCode.Trim().ToUpperInvariant();

        // Soft-deleted countries keep their ISO code reserved by the unique index.
        if (await _context.Countries.IgnoreQueryFilters().AsNoTracking().AnyAsync(c => c.IsoCode == isoCode, cancellationToken))
        {
            return Conflict($"A country with ISO code '{isoCode}' already exists.");
        }

        Country? country = null;
        var failure = ApplyDomainChange(() => country = Country.Create(dto.IsoCode, dto.DefaultCurrency, dto.TimeZone));
        if (failure is not null)
        {
            return failure;
        }

        _context.Countries.Add(country!);
        await _context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = country!.Id }, CountryReadDto.FromEntity(country));
    }

    /// <summary>Updates a country's currency, time zone and active state. Admin only.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    public async Task<ActionResult<CountryReadDto>> Update(int id, UpdateCountryDto dto, CancellationToken cancellationToken)
    {
        var country = await _context.Countries.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (country is null)
        {
            return NotFound();
        }

        var failure = ApplyDomainChange(() =>
        {
            country.Update(dto.DefaultCurrency, dto.TimeZone);

            if (dto.IsActive is { } active && active != country.IsActive)
            {
                if (active)
                {
                    country.Activate();
                }
                else
                {
                    country.Deactivate();
                }
            }
        });
        if (failure is not null)
        {
            return failure;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Ok(CountryReadDto.FromEntity(country));
    }

    /// <summary>Soft-deletes a country together with its cities. Admin only.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var country = await _context.Countries.Include(c => c.Cities).FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (country is null)
        {
            return NotFound();
        }

        // The audit interceptor turns the delete (and the cascade to its cities) into a soft delete.
        _context.Countries.Remove(country);
        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
