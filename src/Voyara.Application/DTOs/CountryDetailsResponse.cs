namespace Voyara.Application.DTOs;

public sealed record CountryDetailsResponse(
    int Id,
    string IsoCode,
    string DefaultCurrency,
    string TimeZone,
    bool IsActive,
    int CitiesCount);