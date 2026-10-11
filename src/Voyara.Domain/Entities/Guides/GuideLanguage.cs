using System;
using Voyara.SharedKernel.Entities;


namespace Voyara.Domain.Entities.Guides
{
    /// <summary>
    /// Represents a language spoken by a guide and the guide's proficiency in that language.
    /// </summary> 
    public sealed class GuideLanguage : AuditableEntity
    {
        /// <summary>
        /// Gets the identifier of the guide profile associated with this language.
        /// </summary>
        public Guid GuideProfileId { get; private set; }

        /// <summary>
        /// Gets the code of the language spoken by the guide (e.g., "en" for English, "es" for Spanish).
        /// </summary>
        public string LanguageCode { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the proficiency level of the guide in the specified language.
        /// </summary>
        public string Proficiency { get; private set; } = string.Empty;

        /// <summary>
        /// Navigation property to the associated guide profile.
        /// </summary>
        public GuideProfile? GuideProfile { get; private set; }
    }
}
