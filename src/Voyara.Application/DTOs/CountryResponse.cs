namespace Voyara.Application.DTOs;

public sealed record CountryResponse(
    int Id,
    string IsoCode,
    string DefaultCurrency,
    string TimeZone,
    bool IsActive);