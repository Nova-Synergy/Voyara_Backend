namespace Voyara.Application.DTOs;

public sealed record CityDetailsResponse(
    int Id,
    int CountryId,
    string CountryIsoCode,
    double Latitude,
    double Longitude,
    bool IsActive);