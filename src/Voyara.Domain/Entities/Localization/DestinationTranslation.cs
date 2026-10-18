using Voyara.Domain.Entities.Catalog;

namespace Voyara.Domain.Entities.Localization
{
    /// <summary>
    /// Represents the localized text of a destination; identified by destination and language code.
    /// It is created and changed only through its <see cref="Destination"/>.
    /// </summary>
    public sealed class DestinationTranslation
    {
        internal const int MaxNameLength = 200;
        internal const int MaxTextLength = 4000;

        /// <summary>
        /// Gets the identifier of the owning destination (part of the key).
        /// </summary>
        public Guid DestinationId { get; private set; }

        /// <summary>
        /// Gets the code of the language of this translation (part of the key).
        /// </summary>
        public string LanguageCode { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the localized name.
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the localized description.
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// Gets the cultural etiquette and tips for visitors.
        /// </summary>
        public string? CultureNotes { get; private set; }

        /// <summary>
        /// Gets the destination this translation belongs to.
        /// </summary>
        public Destination Destination { get; private set; } = null!;

        /// <summary>
        /// Gets the language of this translation.
        /// </summary>
        public Language Language { get; private set; } = null!;

        private DestinationTranslation()
        {
        }

        internal static DestinationTranslation Create(
            Destination destination, string languageCode, string name, string? description, string? cultureNotes)
        {
            ArgumentNullException.ThrowIfNull(destination);
            ArgumentException.ThrowIfNullOrWhiteSpace(languageCode);

            var translation = new DestinationTranslation
            {
                Destination = destination,
                DestinationId = destination.Id,
                LanguageCode = languageCode.Trim().ToLowerInvariant()
            };
            translation.Update(name, description, cultureNotes);
            return translation;
        }

        internal void Update(string name, string? description, string? cultureNotes)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            var trimmedName = name.Trim();
            if (trimmedName.Length > MaxNameLength)
                throw new ArgumentException($"Name cannot exceed {MaxNameLength} characters.", nameof(name));

            Name = trimmedName;
            Description = Normalize(description, nameof(description));
            CultureNotes = Normalize(cultureNotes, nameof(cultureNotes));
        }

        private static string? Normalize(string? text, string parameterName)
        {
            var trimmed = text?.Trim();
            if (string.IsNullOrEmpty(trimmed))
                return null;

            if (trimmed.Length > MaxTextLength)
                throw new ArgumentException($"Text cannot exceed {MaxTextLength} characters.", parameterName);

            return trimmed;
        }
    }
}
