using System.ComponentModel.DataAnnotations;
using Voyara.Domain.Entities.Geography;

namespace Voyara.Application.DTOs.Geography;

/// <summary>A city as returned by the API.</summary>
public sealed class CityReadDto
{
    public int Id { get; init; }
    public int CountryId { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public bool IsActive { get; init; }

    public static CityReadDto FromEntity(City city) => new()
    {
        Id = city.Id,
        CountryId = city.CountryId,
        Latitude = city.Latitude,
        Longitude = city.Longitude,
        IsActive = city.IsActive
    };
}

/// <summary>Creates an active city inside the country of the route.</summary>
public sealed class CreateCityDto
{
    [Range(-90d, 90d)] public double Latitude { get; init; }
    [Range(-180d, 180d)] public double Longitude { get; init; }
}

/// <summary>Updates a city. A null IsActive keeps the current state.</summary>
public sealed class UpdateCityDto
{
    [Range(-90d, 90d)] public double Latitude { get; init; }
    [Range(-180d, 180d)] public double Longitude { get; init; }
    public bool? IsActive { get; init; }
}
