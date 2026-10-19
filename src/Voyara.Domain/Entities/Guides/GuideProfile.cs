using System;
using Voyara.Domain.Enums;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Guides
{
    public sealed class GuideProfile : AggregateRoot
    {
        /// <summary>
        /// Gets the identifier of the user who owns this guide profile.
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// Gets the guide's biography.
        /// </summary>
        public string Bio { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the URL of the guide's profile photo.
        /// </summary>
        public string? PhotoUrl { get; private set; }

        /// <summary>
        /// Gets the guide's years of professional experience.
        /// </summary>
        public int YearsOfExperience { get; private set; }

        /// <summary>
        /// Gets the guide's average rating.
        /// </summary>
        public decimal AverageRating { get; private set; }

        /// <summary>
        /// Gets the total number of reviews received by the guide.
        /// </summary>
        public int ReviewCount { get; private set; }

        /// <summary>
        /// Gets the current approval status of the guide profile.
        /// </summary>
        public GuideStatus Status { get; private set; } = GuideStatus.Pending;

        /// <summary>
        /// Gets the reason the profile was rejected, if applicable.
        /// </summary>
        public string? RejectionReason { get; private set; }

        /// <summary>
        /// Gets the identifier of the city associated with the guide.
        /// </summary>
        public int CityId { get; private set; }

        /// <summary>
        /// Gets the languages spoken by the guide.
        /// </summary>
        public ICollection<GuideLanguage> Languages { get; private set; } = new List<GuideLanguage>();

        /// <summary>
        /// Gets the guide's areas of expertise.
        /// </summary>
        public ICollection<GuideExpertise> Expertise { get; private set; } = new List<GuideExpertise>();

        /// <summary>
        /// Gets the guide's availability schedule.
        /// </summary>
        public ICollection<GuideAvailability> Availability { get; private set; } = new List<GuideAvailability>();

        /// <summary>
        /// Gets the guide's scheduled time-off periods.
        /// </summary>
        public ICollection<GuideTimeOff> TimeOff { get; private set; } = new List<GuideTimeOff>();

        /// <summary>
        /// Gets the guide's pricing options.
        /// </summary>
        public ICollection<GuidePricing> Pricing { get; private set; } = new List<GuidePricing>();

        /// <summary>
        /// Gets the reviews received by the guide.
        /// </summary>
        public ICollection<GuideReview> Reviews { get; private set; } = new List<GuideReview>();

        /// <summary>
        /// Gets the requests associated with this guide profile.
        /// </summary>
        public ICollection<GuideRequest> GuideRequests { get; private set; } = new List<GuideRequest>();
    }
}
