namespace Voyara.Application.DTOs;

/// <summary>Binary content of a stored profile image with its media type.</summary>
public sealed record ProfileImageResponse(Stream Content, string ContentType);
