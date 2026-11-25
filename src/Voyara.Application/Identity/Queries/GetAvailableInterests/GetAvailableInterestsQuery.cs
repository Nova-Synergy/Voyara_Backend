using Voyara.Application.Abstractions.Authorization;
using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.Identity.Queries.GetAvailableInterests;

/// <summary>Lists every interest a tourist can choose from when managing preferences.</summary>
[AuthorizeRequest(Roles.Tourist)]
public sealed record GetAvailableInterestsQuery : IQuery<IReadOnlyList<InterestResponse>>;
