using Voyara.SharedKernel.Entities;
using Voyara.Domain.Enums;

namespace Voyara.Domain.Entities.Identity
{
    /// <summary>
    /// Represents the tourist-specific profile; it is keyed by the identifier of the owning user.
    /// </summary>
    public sealed class TouristProfile : BaseEntity
    {
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
        /// Gets the interests the tourist likes.
        /// </summary>
        public ICollection<Interest> Interests { get; private set; } = new List<Interest>();

        /// <summary>
        /// Gets the destinations the tourist visited before (0..*).
        /// </summary>
        public ICollection<VisitHistory> VisitHistories { get; private set; } = new List<VisitHistory>();
    }
}
