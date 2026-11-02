using System.ComponentModel.DataAnnotations;
using Voyara.Domain.Entities.Geography;

namespace Voyara.Application.DTOs.Geography;

/// <summary>A country as returned by the API.</summary>
public sealed class CountryReadDto
{
    public int Id { get; init; }
    public string IsoCode { get; init; } = string.Empty;
    public string DefaultCurrency { get; init; } = string.Empty;
    public string TimeZone { get; init; } = string.Empty;
    public bool IsActive { get; init; }

    public static CountryReadDto FromEntity(Country country) => new()
    {
        Id = country.Id,
        IsoCode = country.IsoCode,
        DefaultCurrency = country.DefaultCurrency,
        TimeZone = country.TimeZone,
        IsActive = country.IsActive
    };
}

/// <summary>Creates an active country.</summary>
public sealed class CreateCountryDto
{
    [Required, StringLength(2, MinimumLength = 2)] public string IsoCode { get; init; } = string.Empty;
    [Required, StringLength(3, MinimumLength = 3)] public string DefaultCurrency { get; init; } = string.Empty;
    [Required, StringLength(64)] public string TimeZone { get; init; } = string.Empty;
}

/// <summary>Updates a country. The ISO code cannot change; a null IsActive keeps the current state.</summary>
public sealed class UpdateCountryDto
{
    [Required, StringLength(3, MinimumLength = 3)] public string DefaultCurrency { get; init; } = string.Empty;
    [Required, StringLength(64)] public string TimeZone { get; init; } = string.Empty;
    public bool? IsActive { get; init; }
}
