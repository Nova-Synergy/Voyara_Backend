using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs.Identity;

namespace Voyara.Application.Identity.Queries.TourGuides.GetTourGuideById;

/// <summary>
/// Gets a guide profile. Non-approved profiles are visible only to the owner and administrators.
/// </summary>
public sealed record GetTourGuideByIdQuery(Guid Id) : IQuery<TourGuideReadDto>;
