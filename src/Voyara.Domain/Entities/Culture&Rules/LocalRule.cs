using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.Domain.Enums;

namespace Voyara.Domain.Entities.Culture_Rules
{
    /// <summary>
    /// A cultural, legal or behavioral rule that applies to a country, a city or a destination.
    /// The localized text of the rule is stored in <see cref="Translations"/>.
    /// </summary>
    /// <remarks>
    /// The effective rules for a destination are the destination rules plus the rules of its city and
    /// its country. That is query logic and belongs in the application layer, not in the mapping.
    /// </remarks>
    public class LocalRule
    {
        /// <summary>Primary key of the rule.</summary>
        public Guid Id { get; set; }

        /// <summary>The category of the rule.</summary>
        public RuleType Type { get; set; }

        /// <summary>The level (country, city or destination) the rule applies to.</summary>
        public ScopeType Scope { get; set; }

        /// <summary>
        /// Identifier of the entity the rule applies to, stored as text because the referenced entities use
        /// different key types. It holds the <c>int</c> Id of a country or city, or the <c>Guid</c> Id of a destination,
        /// according to <see cref="Scope"/>. No foreign key is created because the target depends on <see cref="Scope"/>.
        /// </summary>
        public string ScopeId { get; set; } = null!;

        /// <summary><see langword="true"/> when the rule comes from an official authority; otherwise it is a custom or cultural norm.</summary>
        public bool IsOfficial { get; set; }

        /// <summary>Where the rule comes from (for example, an authority name, a law reference or a website).</summary>
        public string Source { get; set; } = null!;

        /// <summary>
        /// UTC timestamp when the rule was last reviewed for accuracy. Must have a zero offset
        /// (for example <see cref="DateTimeOffset.UtcNow"/>) because PostgreSQL <c>timestamptz</c> only accepts UTC values.
        /// </summary>
        public DateTimeOffset LastReviewedAt { get; set; }

        /// <summary>
        /// Localized versions of the rule. Composition: translations cannot exist without their rule.
        /// A rule must have at least one translation (1..*); EF Core does not enforce this, so it must be
        /// validated in the application layer.
        /// </summary>
        public ICollection<LocalRuleTranslation> Translations { get; set; } = new List<LocalRuleTranslation>();
    }
}
