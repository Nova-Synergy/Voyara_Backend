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
/// Cities of a country (the country owns its cities, so they live under its route). Reading is public
/// (tourists see active cities of active countries); creating, updating and deleting is for administrators.
/// </summary>
[Route("api/v1/countries/{countryId:int}/cities")]
public class CitiesController : DomainControllerBase
{
    private readonly VoyaraDbContext _context;
    private readonly ICurrentUser _currentUser;

    public CitiesController(VoyaraDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    private bool IsAdmin => _currentUser.IsInRole(Roles.Admin);

    /// <summary>Lists the cities of a country ordered by identifier.</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<CityReadDto>>> GetAll(
        int countryId,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var country = await _context.Countries.AsNoTracking().FirstOrDefaultAsync(c => c.Id == countryId, cancellationToken);
        if (country is null || (!country.IsActive && !IsAdmin))
        {
            return NotFound();
        }

        var query = _context.Cities.AsNoTracking().Where(c => c.CountryId == countryId);
        if (!(includeInactive && IsAdmin))
        {
            query = query.Where(c => c.IsActive);
        }

        var cities = await query.OrderBy(c => c.Id).ToListAsync(cancellationToken);

        return Ok(cities.Select(CityReadDto.FromEntity).ToList());
    }

    /// <summary>Gets a city. Inactive cities and cities of inactive countries are visible to administrators only.</summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<CityReadDto>> GetById(int countryId, int id, CancellationToken cancellationToken)
    {
        var city = await _context.Cities.AsNoTracking()
            .Include(c => c.Country)
            .FirstOrDefaultAsync(c => c.Id == id && c.CountryId == countryId, cancellationToken);

        if (city is null || (!(city.IsActive && city.Country.IsActive) && !IsAdmin))
        {
            return NotFound();
        }

        return Ok(CityReadDto.FromEntity(city));
    }

    /// <summary>Adds an active city to the country. Admin only.</summary>
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    public async Task<ActionResult<CityReadDto>> Create(int countryId, CreateCityDto dto, CancellationToken cancellationToken)
    {
        var country = await _context.Countries.FirstOrDefaultAsync(c => c.Id == countryId, cancellationToken);
        if (country is null)
        {
            return NotFound();
        }

        City? city = null;
        var failure = ApplyDomainChange(() => city = country.AddCity(dto.Latitude, dto.Longitude));
        if (failure is not null)
        {
            return failure;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { countryId, id = city!.Id }, CityReadDto.FromEntity(city));
    }

    /// <summary>Updates a city's coordinates and active state. Admin only.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    public async Task<ActionResult<CityReadDto>> Update(int countryId, int id, UpdateCityDto dto, CancellationToken cancellationToken)
    {
        var city = await _context.Cities.FirstOrDefaultAsync(c => c.Id == id && c.CountryId == countryId, cancellationToken);
        if (city is null)
        {
            return NotFound();
        }

        var failure = ApplyDomainChange(() =>
        {
            city.Relocate(dto.Latitude, dto.Longitude);

            if (dto.IsActive is { } active && active != city.IsActive)
            {
                if (active)
                {
                    city.Activate();
                }
                else
                {
                    city.Deactivate();
                }
            }
        });
        if (failure is not null)
        {
            return failure;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Ok(CityReadDto.FromEntity(city));
    }

    /// <summary>Soft-deletes a city. Admin only.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    public async Task<ActionResult> Delete(int countryId, int id, CancellationToken cancellationToken)
    {
        var city = await _context.Cities.FirstOrDefaultAsync(c => c.Id == id && c.CountryId == countryId, cancellationToken);
        if (city is null)
        {
            return NotFound();
        }

        // The audit interceptor turns the delete into a soft delete.
        _context.Cities.Remove(city);
        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
