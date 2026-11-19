namespace Voyara.Application.DTOs;

public sealed record CityResponse(
    int Id,
    int CountryId,
    double Latitude,
    double Longitude,
    bool IsActive);