using Voyara.SharedKernel.Entities;
using Voyara.Domain.Enums;

namespace Voyara.Domain.Entities.Identity
{
    /// <summary>
    /// Represents the tourist-specific profile of a user. It composes the visit history and holds the
    /// interests the tourist likes; both are changed only through the profile.
    /// </summary>
    public sealed class TouristProfile : BaseEntity
    {
        private const int MaxPhotoUrlLength = 2048;

        private readonly List<Interest> _interests = [];
        private readonly List<VisitHistory> _visitHistories = [];

        /// <summary>
        /// Gets the identifier of the owning user (primary key and foreign key).
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// Gets the two-letter nationality code.
        /// </summary>
        public string NationalityCode { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the code of the preferred display language.
        /// </summary>
        public string PreferredLanguage { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the preferred spending tier.
        /// </summary>
        public BudgetLevel Budget { get; private set; }

        /// <summary>
        /// Gets the optional profile photo location.
        /// </summary>
        public string? PhotoUrl { get; private set; }

        /// <summary>
        /// Gets the user that owns this profile.
        /// </summary>
        public User User { get; private set; } = null!;

        /// <summary>
        /// Read-only view of the liked interests. Changed through <see cref="AddInterest"/>.
        /// </summary>
        public IReadOnlyCollection<Interest> Interests => _interests.AsReadOnly();

        /// <summary>
        /// Read-only view of the destinations the tourist visited before (0..*).
        /// </summary>
        public IReadOnlyCollection<VisitHistory> VisitHistories => _visitHistories.AsReadOnly();

        private TouristProfile()
        {
        }

        /// <summary>Creates the profile of a user.</summary>
        /// <param name="user">The owning user.</param>
        /// <param name="nationalityCode">Two-letter nationality code.</param>
        /// <param name="preferredLanguage">Preferred display language code.</param>
        /// <param name="budget">Preferred spending tier.</param>
        /// <param name="photoUrl">Optional photo location.</param>
        /// <returns>The created <see cref="TouristProfile"/> instance.</returns>
        /// <exception cref="ArgumentException">An argument is missing or malformed.</exception>
        public static TouristProfile Create(
            User user, string nationalityCode, string preferredLanguage, BudgetLevel budget, string? photoUrl = null)
        {
            ArgumentNullException.ThrowIfNull(user);

            return new TouristProfile
            {
                User = user,
                UserId = user.Id,
                NationalityCode = NormalizeNationality(nationalityCode),
                PreferredLanguage = NormalizeLanguage(preferredLanguage),
                Budget = EnsureBudget(budget),
                PhotoUrl = NormalizePhoto(photoUrl)
            };
        }

        /// <summary>Changes the nationality.</summary>
        public void ChangeNationality(string nationalityCode) => NationalityCode = NormalizeNationality(nationalityCode);

        /// <summary>Changes the preferred display language.</summary>
        public void ChangePreferredLanguage(string preferredLanguage) => PreferredLanguage = NormalizeLanguage(preferredLanguage);

        /// <summary>Changes the preferred spending tier.</summary>
        public void ChangeBudget(BudgetLevel budget) => Budget = EnsureBudget(budget);

        /// <summary>Sets or clears (null) the profile photo.</summary>
        public void ChangePhoto(string? photoUrl) => PhotoUrl = NormalizePhoto(photoUrl);

        /// <summary>Adds an interest; adding one the tourist already likes has no effect.</summary>
        public void AddInterest(Interest interest)
        {
            ArgumentNullException.ThrowIfNull(interest);

            if (_interests.All(i => i.Id != interest.Id || interest.Id == 0))
                _interests.Add(interest);
        }

        /// <summary>
        /// Removes an interest; removing one the tourist does not like has no effect.
        /// </summary>
        public void RemoveInterest(Interest interest)
        {
            ArgumentNullException.ThrowIfNull(interest);
            _interests.RemoveAll(i => i.Id == interest.Id);
        }

        /// <summary>Records a visit. A visit cannot be in the future and the same visit is recorded once.</summary>
        /// <param name="destinationId">The visited destination.</param>
        /// <param name="visitedOn">The day of the visit.</param>
        /// <param name="now">The current time, used to reject future visits.</param>
        /// <exception cref="InvalidOperationException">The visit is in the future or already recorded.</exception>
        public VisitHistory RecordVisit(Guid destinationId, DateTime visitedOn, DateTimeOffset now)
        {
            if (visitedOn.Date > now.UtcDateTime.Date)
                throw new InvalidOperationException("A visit cannot be recorded in the future.");

            if (_visitHistories.Any(v => v.DestinationId == destinationId && v.VisitedOn == visitedOn.Date))
                throw new InvalidOperationException("That visit was already recorded.");

            var visit = VisitHistory.Create(this, destinationId, visitedOn);
            _visitHistories.Add(visit);
            return visit;
        }

        /// <summary>Removes a recorded visit.</summary>
        /// <exception cref="InvalidOperationException">The profile has no such visit.</exception>
        public void RemoveVisit(Guid visitId)
        {
            var visit = _visitHistories.Find(v => v.Id == visitId)
                ?? throw new InvalidOperationException("The visit was not found.");

            _visitHistories.Remove(visit);
        }

        private static string NormalizeNationality(string code)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(code);

            var normalized = code.Trim().ToUpperInvariant();
            if (normalized.Length != 2 || !normalized.All(char.IsAsciiLetterUpper))
                throw new ArgumentException("Nationality must be a two-letter country code.", nameof(code));

            return normalized;
        }

        private static string NormalizeLanguage(string code)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(code);
            return code.Trim().ToLowerInvariant();
        }

        private static BudgetLevel EnsureBudget(BudgetLevel budget)
            => Enum.IsDefined(budget)
                ? budget
                : throw new ArgumentOutOfRangeException(nameof(budget), "Budget level is not valid.");

        private static string? NormalizePhoto(string? photoUrl)
        {
            var normalized = photoUrl?.Trim();
            if (string.IsNullOrEmpty(normalized))
                return null;

            if (normalized.Length > MaxPhotoUrlLength)
                throw new ArgumentException($"Photo location cannot exceed {MaxPhotoUrlLength} characters.", nameof(photoUrl));

            return normalized;
        }
    }
}
