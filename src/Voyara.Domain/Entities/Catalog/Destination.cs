using Voyara.Domain.Entities.Geography;
using Voyara.Domain.Enums;
using Voyara.Domain.Events.Catalog;
using Voyara.Domain.ValueObjects;
using Voyara.SharedKernel.Entities;
using Voyara.Domain.Entities.Localization;

namespace Voyara.Domain.Entities.Catalog
{
    /// <summary>
    /// Aggregate root representing a tourist destination in the catalog. Its translations, gallery, opening hours,
    /// opening exceptions and ticket prices are composed by the destination: they are created and changed only
    /// through the methods of this class, which also protect the aggregate's rules.
    /// </summary>
    public sealed class Destination : AggregateRoot
    {
        private const int MaxVisitMinutes = 24 * 60;

        private readonly List<DestinationImage> _images = [];
        private readonly List<OpeningHour> _openingHours = [];
        private readonly List<OpeningException> _openingExceptions = [];
        private readonly List<TicketPrice> _ticketPrices = [];
        private readonly List<DestinationTranslation> _translations = [];

        /// <summary>
        /// Gets the geographic location of the destination.
        /// </summary>
        public GeoPoint Location { get; private set; } = null!;

        /// <summary>
        /// Gets the typical time a visitor spends at the destination, in minutes.
        /// </summary>
        public int EstimatedVisitMinutes { get; private set; }

        /// <summary>
        /// Gets the average review rating (0..5).
        /// </summary>
        public double AverageRating { get; private set; }

        /// <summary>
        /// Gets the number of reviews behind <see cref="AverageRating"/>.
        /// </summary>
        public int ReviewCount { get; private set; }

        /// <summary>
        /// Gets the score used to rank destinations in search results.
        /// </summary>
        public int PopularityScore { get; private set; }

        /// <summary>
        /// Gets the current lifecycle status.
        /// </summary>
        public DestinationStatus Status { get; private set; } = DestinationStatus.Draft;

        /// <summary>
        /// Gets the identifier of the city the destination is located in.
        /// </summary>
        public Guid CityId { get; private set; }

        /// <summary>
        /// Gets the city the destination is located in.
        /// </summary>
        public City City { get; private set; } = null!;

        /// <summary>
        /// Gets the identifier of the category classifying the destination.
        /// </summary>
        public Guid CategoryId { get; private set; }

        /// <summary>
        /// Gets the category classifying the destination.
        /// </summary>
        public Category Category { get; private set; } = null!;

        /// <summary>
        /// Read-only view of the gallery pictures (0..*). Changed through <see cref="AddImage"/> and friends.
        /// </summary>
        public IReadOnlyCollection<DestinationImage> Images => _images.AsReadOnly();

        /// <summary>
        /// Read-only view of the regular weekly opening hours (0..*).
        /// </summary>
        public IReadOnlyCollection<OpeningHour> OpeningHours => _openingHours.AsReadOnly();

        /// <summary>
        /// Read-only view of the date-specific opening overrides (0..*).
        /// </summary>
        public IReadOnlyCollection<OpeningException> OpeningExceptions => _openingExceptions.AsReadOnly();

        /// <summary>
        /// Read-only view of the ticket prices per visitor type (0..*).
        /// </summary>
        public IReadOnlyCollection<TicketPrice> TicketPrices => _ticketPrices.AsReadOnly();

        /// <summary>
        /// Read-only view of the localized texts (1..*).
        /// </summary>
        public IReadOnlyCollection<DestinationTranslation> Translations => _translations.AsReadOnly();

        private Destination()
        {
        }

        /// <summary>
        /// Creates a destination in <see cref="DestinationStatus.Draft"/> together with its first translation,
        /// so the "at least one translation" rule holds from the start.
        /// </summary>
        /// <param name="location">Geographic location.</param>
        /// <param name="cityId">Identifier of the city the destination is in.</param>
        /// <param name="categoryId">Identifier of the category.</param>
        /// <param name="estimatedVisitMinutes">Typical visit time, 1 to 1440 minutes.</param>
        /// <param name="languageCode">Language of the first translation.</param>
        /// <param name="name">Localized name.</param>
        /// <param name="description">Localized description.</param>
        /// <param name="cultureNotes">Localized cultural tips.</param>
        /// <returns>The created <see cref="Destination"/> instance.</returns>
        /// <exception cref="ArgumentException">An argument is missing or out of range.</exception>
        public static Destination Create(
            GeoPoint location,
            Guid cityId,
            Guid categoryId,
            int estimatedVisitMinutes,
            string languageCode,
            string name,
            string? description,
            string? cultureNotes)
        {
            ArgumentNullException.ThrowIfNull(location);
            EnsureId(cityId, nameof(cityId));
            EnsureId(categoryId, nameof(categoryId));
            EnsureVisitMinutes(estimatedVisitMinutes);

            var destination = new Destination
            {
                Location = location,
                CityId = cityId,
                CategoryId = categoryId,
                EstimatedVisitMinutes = estimatedVisitMinutes
            };

            destination._translations.Add(
                DestinationTranslation.Create(destination, languageCode, name, description, cultureNotes));
            return destination;
        }

        // ------------------------------------------------------------------ lifecycle

        /// <summary>Makes a draft destination publicly visible.</summary>
        /// <exception cref="InvalidOperationException">The destination is not a draft.</exception>
        public void Publish(DateTimeOffset now)
        {
            if (Status != DestinationStatus.Draft)
                throw new InvalidOperationException("Only a draft destination can be published.");

            Status = DestinationStatus.Published;
            RaiseDomainEvent(new DestinationPublishedDomainEvent(Guid.NewGuid(), now, Id, _translations[0].Name));
        }

        /// <summary>Retires the destination from the public catalog (soft delete); it is kept for history.</summary>
        /// <exception cref="InvalidOperationException">The destination is already archived.</exception>
        public void Archive()
        {
            if (Status == DestinationStatus.Archived)
                throw new InvalidOperationException("The destination is already archived.");

            Status = DestinationStatus.Archived;
        }

        // ------------------------------------------------------------------ core details

        /// <summary>Moves the destination to new coordinates.</summary>
        public void Relocate(GeoPoint location)
        {
            EnsureEditable();
            ArgumentNullException.ThrowIfNull(location);
            Location = location;
        }

        /// <summary>Places the destination in another city.</summary>
        public void MoveToCity(Guid cityId)
        {
            EnsureEditable();
            EnsureId(cityId, nameof(cityId));
            CityId = cityId;
        }

        /// <summary>Classifies the destination under another category.</summary>
        public void ChangeCategory(Guid categoryId)
        {
            EnsureEditable();
            EnsureId(categoryId, nameof(categoryId));
            CategoryId = categoryId;
        }

        /// <summary>Changes the typical visit duration.</summary>
        public void ChangeEstimatedVisitTime(int minutes)
        {
            EnsureEditable();
            EnsureVisitMinutes(minutes);
            EstimatedVisitMinutes = minutes;
        }

        /// <summary>Applies the review statistics calculated by the reviews module.</summary>
        public void ApplyReviewStatistics(double averageRating, int reviewCount)
        {
            if (double.IsNaN(averageRating) || averageRating is < 0 or > 5)
                throw new ArgumentOutOfRangeException(nameof(averageRating), "Rating must be between 0 and 5.");

            ArgumentOutOfRangeException.ThrowIfNegative(reviewCount);

            AverageRating = averageRating;
            ReviewCount = reviewCount;
        }

        /// <summary>Applies the ranking score calculated by the recommendation module.</summary>
        public void ChangePopularity(int score)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(score);
            PopularityScore = score;
        }

        // ------------------------------------------------------------------ translations

        /// <summary>Adds the text of a language, or replaces it when that language already has one.</summary>
        public void SetTranslation(string languageCode, string name, string? description, string? cultureNotes)
        {
            EnsureEditable();
            ArgumentException.ThrowIfNullOrWhiteSpace(languageCode);

            var code = languageCode.Trim().ToLowerInvariant();
            var existing = _translations.Find(t => t.LanguageCode == code);
            if (existing is not null)
                existing.Update(name, description, cultureNotes);
            else
                _translations.Add(DestinationTranslation.Create(this, code, name, description, cultureNotes));
        }

        /// <summary>Removes the text of a language; the last remaining translation cannot be removed.</summary>
        /// <exception cref="InvalidOperationException">The language has no translation or it is the last one.</exception>
        public void RemoveTranslation(string languageCode)
        {
            EnsureEditable();

            var code = languageCode?.Trim().ToLowerInvariant();
            var existing = _translations.Find(t => t.LanguageCode == code)
                ?? throw new InvalidOperationException("The destination has no translation in that language.");

            if (_translations.Count == 1)
                throw new InvalidOperationException("A destination must keep at least one translation.");

            _translations.Remove(existing);
        }

        /// <summary>
        /// Returns the translation in the requested language, falling back to English and then to the first
        /// available translation.
        /// </summary>
        public DestinationTranslation GetTranslation(string? languageCode = null)
        {
            var code = languageCode?.Trim().ToLowerInvariant();

            return _translations.Find(t => t.LanguageCode == code)
                   ?? _translations.Find(t => t.LanguageCode == "en")
                   ?? _translations[0];
        }

        // ------------------------------------------------------------------ gallery

        /// <summary>Adds a picture at the end of the gallery; the first picture always becomes the cover.</summary>
        public DestinationImage AddImage(string url, bool makeCover = false)
        {
            EnsureEditable();

            var isCover = makeCover || _images.Count == 0;
            var image = DestinationImage.Create(this, url, _images.Count, isCover);

            if (isCover)
                _images.ForEach(i => i.MarkAsCover(false));

            _images.Add(image);
            return image;
        }

        /// <summary>Makes an existing picture the single cover image.</summary>
        /// <exception cref="InvalidOperationException">The destination has no such image.</exception>
        public void SetCoverImage(Guid imageId)
        {
            EnsureEditable();

            if (!_images.Any(i => i.Id == imageId))
                throw new InvalidOperationException("The destination has no such image.");

            _images.ForEach(i => i.MarkAsCover(i.Id == imageId));
        }

        /// <summary>Removes a picture; the first remaining picture becomes the cover when the cover was removed.</summary>
        /// <exception cref="InvalidOperationException">The destination has no such image.</exception>
        public void RemoveImage(Guid imageId)
        {
            EnsureEditable();

            var image = _images.Find(i => i.Id == imageId)
                ?? throw new InvalidOperationException("The destination has no such image.");

            _images.Remove(image);
            for (var index = 0; index < _images.Count; index++)
                _images[index].MoveTo(index);

            if (image.IsCover && _images.Count > 0)
                _images[0].MarkAsCover(true);
        }

        // ------------------------------------------------------------------ opening hours

        /// <summary>Sets the regular opening hours of a weekday, replacing any previous value for that day.</summary>
        public void SetOpeningHours(DayOfWeek day, TimeSpan opensAt, TimeSpan closesAt)
            => GetOrAddHours(day).Open(opensAt, closesAt);

        /// <summary>Marks a weekday as closed all day.</summary>
        public void SetClosedOn(DayOfWeek day) => GetOrAddHours(day).Close();

        /// <summary>Adds or replaces the override for a specific calendar date.</summary>
        public void SetOpeningException(DateOnly date, bool isClosed, string reason)
        {
            EnsureEditable();

            var existing = _openingExceptions.Find(e => e.Date == date);
            if (existing is not null)
                existing.Update(isClosed, reason);
            else
                _openingExceptions.Add(OpeningException.Create(this, date, isClosed, reason));
        }

        /// <summary>Removes the override of a calendar date.</summary>
        /// <exception cref="InvalidOperationException">The destination has no override on that date.</exception>
        public void RemoveOpeningException(DateOnly date)
        {
            EnsureEditable();

            var existing = _openingExceptions.Find(e => e.Date == date)
                ?? throw new InvalidOperationException("The destination has no opening exception on that date.");

            _openingExceptions.Remove(existing);
        }

        /// <summary>
        /// Returns whether the destination is open at the given local date and time. A closing exception on that
        /// date wins; otherwise the weekly hours apply, including venues that stay open past midnight.
        /// </summary>
        public bool IsOpenAt(DateTime dateTime)
        {
            var date = DateOnly.FromDateTime(dateTime);
            var time = dateTime.TimeOfDay;

            if (_openingExceptions.Any(e => e.Date == date && e.IsClosed))
                return false;

            var today = _openingHours.Find(h => h.Day == dateTime.DayOfWeek);
            if (today is { IsClosed: false })
            {
                var opensEarlierThanItCloses = today.OpensAt <= today.ClosesAt;
                if (time >= today.OpensAt && (!opensEarlierThanItCloses || time < today.ClosesAt))
                    return true;
            }

            var yesterday = _openingHours.Find(h => h.Day == dateTime.AddDays(-1).DayOfWeek);
            return yesterday is { IsClosed: false } && yesterday.ClosesAt < yesterday.OpensAt && time < yesterday.ClosesAt;
        }

        // ------------------------------------------------------------------ ticket prices

        /// <summary>Sets the ticket price of a visitor type, replacing any previous value for that type.</summary>
        public void SetTicketPrice(VisitorType visitorType, Money price, string? notes = null)
        {
            EnsureEditable();

            var existing = _ticketPrices.Find(t => t.VisitorType == visitorType);
            if (existing is not null)
                existing.Update(price, notes);
            else
                _ticketPrices.Add(TicketPrice.Create(this, visitorType, price, notes));
        }

        /// <summary>Removes the ticket price of a visitor type.</summary>
        /// <exception cref="InvalidOperationException">The destination has no price for that visitor type.</exception>
        public void RemoveTicketPrice(VisitorType visitorType)
        {
            EnsureEditable();

            var existing = _ticketPrices.Find(t => t.VisitorType == visitorType)
                ?? throw new InvalidOperationException("The destination has no ticket price for that visitor type.");

            _ticketPrices.Remove(existing);
        }

        // ------------------------------------------------------------------ helpers

        private OpeningHour GetOrAddHours(DayOfWeek day)
        {
            EnsureEditable();

            var existing = _openingHours.Find(h => h.Day == day);
            if (existing is not null)
                return existing;

            var created = OpeningHour.Create(this, day);
            _openingHours.Add(created);
            return created;
        }

        private void EnsureEditable()
        {
            if (Status == DestinationStatus.Archived)
                throw new InvalidOperationException("An archived destination cannot be modified.");
        }

        private static void EnsureId(Guid id, string parameterName)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Identifier is required.", parameterName);
        }

        private static void EnsureVisitMinutes(int minutes)
        {
            if (minutes is < 1 or > MaxVisitMinutes)
                throw new ArgumentOutOfRangeException(nameof(minutes), "Estimated visit time must be between 1 and 1440 minutes.");
        }
    }
}