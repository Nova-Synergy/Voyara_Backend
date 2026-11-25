namespace Voyara.Api.Contracts.Identity;

/// <summary>Request body for updating the tourist's personal details.</summary>
public sealed record UpdateTouristProfileRequest(string FullName, string NationalityCode);
