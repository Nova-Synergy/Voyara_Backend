using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyara.Domain.Entities.Culture_Rules
{

    /// <summary>
    /// The text of a <see cref="LocalRule"/> in one language.
    /// </summary>
    /// <remarks>
    /// Identified by the composite key (<see cref="LocalRuleId"/>, <see cref="LanguageCode"/>),
    /// so a rule has at most one translation per language.
    /// </remarks>
    public class LocalRuleTranslation
    {
        /// <summary>Foreign key to the <see cref="Culture_Rules.LocalRule"/> this translation belongs to. Part of the primary key.</summary>
        public Guid LocalRuleId { get; set; }

        /// <summary>The rule this translation belongs to.</summary>
        public LocalRule LocalRule { get; set; } = null!;

        /// <summary>Language code of the translation, such as <c>ar</c>, <c>en</c> or <c>ar-EG</c>. Part of the primary key.</summary>
        public string LanguageCode { get; set; } = null!;

        /// <summary>Short title of the rule in this language.</summary>
        public string Title { get; set; } = null!;

        /// <summary>Full description of the rule in this language.</summary>
        public string Content { get; set; } = null!;
    }
}
