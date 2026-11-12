using System;
using Voyara.Domain.Entities.Geography;
using Voyara.Domain.Entities.Identity;
using Voyara.Domain.Enums;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Guides
{
    /// <summary>
    /// represents a tour guide's profile, including personal details, professional experience, availability, pricing, and reviews.
    /// </summary>
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
        private readonly List<GuideLanguage> _languages = new();

        /// <summary>
        /// Gets a read-only collection of the guide's languages.
        /// </summary>
        public IReadOnlyCollection<GuideLanguage> Languages => _languages.AsReadOnly();

        /// <summary>
        /// Gets the areas of expertise associated with the guide.
        /// </summary>
        private readonly List<GuideExpertise> _expertise = new();

        /// <summary>
        /// Gets a read-only collection of the guide's areas of expertise.
        /// </summary>
        public IReadOnlyCollection<GuideExpertise> Expertise => _expertise.AsReadOnly();

        /// <summary>
        /// Gets the guide's recurring availability ranges.
        /// </summary>
        private readonly List<GuideAvailability> _availability = new();

        /// <summary>
        /// Gets a read-only collection of the guide's availability ranges.
        /// </summary>
        public IReadOnlyCollection<GuideAvailability> Availability => _availability.AsReadOnly();

        /// <summary>
        /// Gets the dates on which the guide is unavailable.
        /// </summary>
        private readonly List<GuideTimeOff> _timeOff = new();

        /// <summary>
        /// Gets a read-only collection of the guide's time-off records.
        /// </summary>
        public IReadOnlyCollection<GuideTimeOff> TimeOff => _timeOff.AsReadOnly();

        /// <summary>
        /// Gets the pricing options offered by the guide.
        /// </summary>
        private readonly List<GuidePricing> _pricing = new();

        /// <summary>
        /// Gets a read-only collection of the guide's pricing options.
        /// </summary>
        public IReadOnlyCollection<GuidePricing> Pricing => _pricing.AsReadOnly();

        /// <summary>
        /// Gets the reviews associated with the guide's profile.
        /// </summary>
        private readonly List<GuideReview> _reviews = new();

        /// <summary>
        /// Gets a read-only collection of the guide's reviews.
        /// </summary>
        public IReadOnlyCollection<GuideReview> Reviews => _reviews.AsReadOnly();

        /// <summary>
        /// Gets the requests associated with the guide's profile.
        /// </summary>
        private readonly List<GuideRequest> _guideRequests = new();

        /// <summary>
        /// Gets a read-only collection of the guide's requests.
        /// </summary>
        public IReadOnlyCollection<GuideRequest> GuideRequests => _guideRequests.AsReadOnly();

        /// <summary>
        /// Gets the city associated with the guide's profile, if available.
        /// </summary>
        public City? City { get; private set; }

        /// <summary>
        /// Gets the user account associated with the guide's profile, if available.
        /// </summary>
        public User? User { get; private set; }

        /// <summary>
        /// Creates a new guide profile with the specified personal and professional details.
        /// The profile is initialized with a pending approval status.
        /// </summary>
        /// <param name="userId">The identifier of the user who owns the profile.</param>
        /// <param name="cityId">The identifier of the city where the guide operates.</param>
        /// <param name="bio">A description of the guide's background and experience.</param>
        /// <param name="photoUrl">The optional URL of the guide's profile photo.</param>
        /// <param name="yearsOfExperience">The number of years of guiding experience.</param>
        /// <returns>A newly created guide profile with a pending status.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the city identifier or profile details are invalid.
        /// </exception>
        public static GuideProfile Create(
            Guid userId,
            int cityId,
            string bio,
            string? photoUrl,
            int yearsOfExperience)
        {

            if (userId == Guid.Empty)
                throw new ArgumentException("A user is required.", nameof(userId));

            if (cityId <= 0)
                throw new ArgumentException("A city is required.", nameof(cityId));

            ValidateDetails(bio, photoUrl, yearsOfExperience);

            return new GuideProfile
            {
                UserId = userId,
                CityId = cityId,
                Bio = bio.Trim(),
                PhotoUrl = photoUrl,
                YearsOfExperience = yearsOfExperience,
                Status = GuideStatus.Pending
            };
        }

        /// <summary>
        /// Updates the guide's personal and professional profile details.
        /// </summary>
        /// <param name="bio">The updated description of the guide.</param>
        /// <param name="photoUrl">The updated profile photo URL, if available.</param>
        /// <param name="yearsOfExperience">The updated years of guiding experience.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when any of the supplied profile details are invalid.
        /// </exception>
        public void UpdateDetails(string bio, string? photoUrl, int yearsOfExperience)
        {
            ValidateDetails(bio, photoUrl, yearsOfExperience);
            Bio = bio.Trim();
            PhotoUrl = photoUrl;
            YearsOfExperience = yearsOfExperience;
        }

        /// <summary>
        /// Approves a pending guide profile, allowing the guide to appear in the directory
        /// and receive requests.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the profile is not in the pending status.
        /// </exception>
        public void Approve()
        {
            if (Status != GuideStatus.Pending)
                throw new InvalidOperationException($"Only pending profiles can be approved. Current status: '{Status}'.");

            Status = GuideStatus.Approved;
            RejectionReason = null;
        }

        /// <summary>
        /// Rejects a pending guide profile and records the reason communicated to the guide.
        /// </summary>
        /// <param name="reason">The reason for rejecting the profile.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the profile is not in the pending status.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the rejection reason is empty or exceeds 500 characters.
        /// </exception>
        public void Reject(string reason)
        {
            if (Status != GuideStatus.Pending)
                throw new InvalidOperationException(
                    $"Only pending profiles can be rejected. Current status: '{Status}'.");

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("A rejection reason is required.", nameof(reason));

            if (reason.Trim().Length > 500)
                throw new ArgumentException("Reason cannot exceed 500 characters.", nameof(reason));

            Status = GuideStatus.Rejected;
            RejectionReason = reason.Trim();
        }

        /// <summary>
        /// Suspends an approved guide profile, preventing the guide from receiving new requests.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the profile is not in the approved status.
        /// </exception>
        public void Suspend()
        {
            if (Status != GuideStatus.Approved)
                throw new InvalidOperationException($"Only approved profiles can be suspended. Current status: '{Status}'.");

            Status = GuideStatus.Suspended;
        }

        /// <summary>
        /// Adds a language to the guide's profile.
        /// </summary>
        /// <param name="languageCode">The code identifying the language.</param>
        /// <param name="proficiency">The guide's proficiency level in the language.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the language has already been added.
        /// </exception>
        public void AddLanguage(string languageCode, string proficiency)
        {
            if (_languages.Any(l =>
                l.LanguageCode.Equals(languageCode?.Trim(),
                StringComparison.OrdinalIgnoreCase)))

                throw new InvalidOperationException($"Language '{languageCode}' is already added.");

            _languages.Add(GuideLanguage.Create(Id, languageCode!, proficiency));
        }

        /// <summary>
        /// Removes a language from the guide's profile.
        /// </summary>
        /// <param name="languageCode">The code identifying the language to remove.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the specified language is not found.
        /// </exception>
        public void RemoveLanguage(string languageCode)
        {
            var language = _languages.FirstOrDefault(l =>
                l.LanguageCode.Equals(languageCode?.Trim(), StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException($"Language '{languageCode}' was not found.");

            _languages.Remove(language);
        }

        /// <summary>
        /// Adds an area of expertise to the guide's profile.
        /// </summary>
        /// <param name="categoryId">The identifier of the expertise category.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the expertise has already been added.
        /// </exception>
        public void AddExpertise(int categoryId)
        {
            if (_expertise.Any(e => e.CategoryId == categoryId))
                throw new InvalidOperationException("This expertise is already added.");

            _expertise.Add(GuideExpertise.Create(Id, categoryId));
        }

        /// <summary>
        /// Removes an area of expertise from the guide's profile.
        /// </summary>
        /// <param name="categoryId">The identifier of the expertise category to remove.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the specified expertise is not found.
        /// </exception>
        public void RemoveExpertise(int categoryId)
        {
            var item = _expertise.FirstOrDefault(e => e.CategoryId == categoryId)
                ?? throw new InvalidOperationException("Expertise was not found.");

            _expertise.Remove(item);
        }

        /// <summary>
        /// Adds a recurring availability range for a specific day of the week.
        /// </summary>
        /// <param name="day">The day of the week for the availability range.</param>
        /// <param name="start">The start time of the availability range.</param>
        /// <param name="end">The end time of the availability range.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the new range overlaps an existing availability range.
        /// </exception>
        public void AddAvailability(DayOfWeek day, TimeOnly start, TimeOnly end)
        {
            var candidate = GuideAvailability.Create(Id, day, start, end);

            if (_availability.Any(a =>
                a.Day == day && start < a.EndTime && a.StartTime < end))
                throw new InvalidOperationException("This time range overlaps an existing availability range.");

            _availability.Add(candidate);
        }

        /// <summary>
        /// Removes an availability range identified by its day and start time.
        /// </summary>
        /// <param name="day">The day of the week of the availability range.</param>
        /// <param name="start">The start time of the availability range to remove.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the specified availability range is not found.
        /// </exception>
        public void RemoveAvailability(DayOfWeek day, TimeOnly start)
        {
            var item = _availability.FirstOrDefault(a =>
                a.Day == day && a.StartTime == start)
                ?? throw new InvalidOperationException("Availability range was not found.");

            _availability.Remove(item);
        }

        /// <summary>
        /// Adds a time-off record for a specific date.
        /// </summary>
        /// <param name="date">The date on which the guide is unavailable.</param>
        /// <param name="reason">The optional reason for the time off.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a time-off record already exists for the specified date.
        /// </exception>
        public void AddTimeOff(DateOnly date, string? reason)
        {
            if (_timeOff.Any(t => t.Date == date))
                throw new InvalidOperationException("Time off is already set for this date.");

            _timeOff.Add(GuideTimeOff.Create(Id, date, reason));
        }

        /// <summary>
        /// Removes a time-off record for the specified date.
        /// </summary>
        /// <param name="date">The date of the time-off record to remove.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when no time-off record exists for the specified date.
        /// </exception>
        public void RemoveTimeOff(DateOnly date)
        {
            var item = _timeOff.FirstOrDefault(t => t.Date == date)
                ?? throw new InvalidOperationException("Time off was not found.");

            _timeOff.Remove(item);
        }

        /// <summary>
        /// Adds a pricing option for a specific pricing unit and currency.
        /// </summary>
        /// <param name="unit">The unit used to calculate the price.</param>
        /// <param name="amount">The price amount for the selected unit.</param>
        /// <param name="currency">The currency code for the price.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a price for the same unit and currency already exists.
        /// </exception>
        public void AddPricing(PricingUnit unit, decimal amount, string currency)
        {
            if (_pricing.Any(p =>
                p.Unit == unit &&
                p.Currency.Equals(currency?.Trim(), StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("A price for this unit and currency already exists. Remove it first.");

            _pricing.Add(GuidePricing.Create(Id, unit, amount, currency!));
        }

        /// <summary>
        /// Removes a pricing option identified by its unit and currency.
        /// </summary>
        /// <param name="unit">The pricing unit of the option to remove.</param>
        /// <param name="currency">The currency code of the option to remove.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the specified pricing option is not found.
        /// </exception>
        public void RemovePricing(PricingUnit unit, string currency)
        {
            var item = _pricing.FirstOrDefault(p =>
                p.Unit == unit &&
                p.Currency.Equals(currency?.Trim(), StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException("Price was not found.");

            _pricing.Remove(item);
        }

        /// <summary>
        /// Updates the guide's average rating and review count using a newly submitted rating.
        /// </summary>
        /// <param name="rating">The rating value, from 1 to 5, to apply to the profile.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the rating is outside the range of 1 to 5.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the guide's profile is not approved.
        /// </exception>
        public void ApplyReview(int rating)
        {
            if (rating is < 1 or > 5)
                throw new ArgumentException("Rating must be between 1 and 5.", nameof(rating));

            if (Status != GuideStatus.Approved)
                throw new InvalidOperationException("Only approved guides can receive reviews.");

            var total = AverageRating * ReviewCount + rating;
            ReviewCount++;
            AverageRating = Math.Round(total / ReviewCount, 2);
        }

        /// <summary>
        /// Validates the guide's profile details before creation or update.
        /// </summary>
        /// <param name="bio">The guide's biography.</param>
        /// <param name="photoUrl">The optional profile photo URL.</param>
        /// <param name="years">The guide's years of experience.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the biography is missing or too long, the photo URL exceeds
        /// the allowed length, or the years of experience are negative.
        /// </exception>
        private static void ValidateDetails(string bio,string? photoUrl,int years)
        {
            if (string.IsNullOrWhiteSpace(bio))
                throw new ArgumentException("Bio is required.", nameof(bio));

            if (bio.Trim().Length > 1000)
                throw new ArgumentException("Bio cannot exceed 1000 characters.", nameof(bio));

            if (photoUrl is { Length: > 2048 })
                throw new ArgumentException("Photo URL is too long.", nameof(photoUrl));

            if (years < 0)
                throw new ArgumentException("Years of experience cannot be negative.", nameof(years));
        }
    }
}

