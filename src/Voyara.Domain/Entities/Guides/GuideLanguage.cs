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

        /// <summary>
        /// Initializes a new instance of the GuideLanguage class. 
        /// </summary>
        /// <remarks>This constructor is private and is intended for internal use only. Instances of
        /// GuideLanguage cannot be created directly by consumers.</remarks>
        private GuideLanguage()
        {
        }

        /// <summary>
        /// Creates a new guide language associated with a guide profile.
        /// </summary>
        /// <param name="guideProfileId">
        /// The unique identifier of the guide profile.
        /// </param>
        /// <param name="languageCode">
        /// The language code, which must not be empty or exceed 10 characters.
        /// </param>
        /// <param name="proficiency">
        /// The guide's proficiency level in the specified language, which must not be empty or exceed 50 characters.
        /// </param>
        /// <returns>
        /// A new instance of the <see cref="GuideLanguage"/> class with validated and normalized values.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="guideProfileId"/> is <see cref="Guid.Empty"/>,
        /// <paramref name="languageCode"/> is null, empty, or exceeds 10 characters,
        /// or <paramref name="proficiency"/> is null, empty, or exceeds 50 characters.
        /// </exception>
        internal static GuideLanguage Create(Guid guideProfileId, string languageCode, string proficiency)
        {
            if (guideProfileId == Guid.Empty)
                throw new ArgumentException("A guide profile is required.", nameof(guideProfileId));

            if (string.IsNullOrWhiteSpace(languageCode) || languageCode.Trim().Length > 10)
                throw new ArgumentException("Language code is required (max 10 characters).", nameof(languageCode));

            if (string.IsNullOrWhiteSpace(proficiency) || proficiency.Trim().Length > 50)
                throw new ArgumentException("Proficiency is required (max 50 characters).", nameof(proficiency));

            return new GuideLanguage
            {
                GuideProfileId = guideProfileId,
                LanguageCode = languageCode.Trim().ToLowerInvariant(),
                Proficiency = proficiency.Trim()
            };
        }
    }
}