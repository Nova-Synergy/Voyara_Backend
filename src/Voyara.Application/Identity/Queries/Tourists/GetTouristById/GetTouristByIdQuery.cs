using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs.Identity;

namespace Voyara.Application.Identity.Queries.Tourists.GetTouristById;

/// <summary>Gets a tourist profile by user id. Owner or admin.</summary>
[AuthorizeRequest]
public sealed record GetTouristByIdQuery(Guid UserId) : IQuery<TouristReadDto>;
