using System.Text.RegularExpressions;

namespace Voyara.Domain.Entities.Localization
{
    /// <summary>
    /// Represents a supported content language, identified by its code (for example en, ar).
    /// </summary>
    public sealed partial class Language
    {
        /// <summary>
        /// Gets the language code; it is the primary key.
        /// </summary>
        public string Code { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the display name of the language.
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether the language is written right-to-left.
        /// </summary>
        public bool IsRtl { get; private set; }

        private Language()
        {
        }

        /// <summary>Creates a supported language.</summary>
        /// <param name="code">Language code such as en, ar or pt-br.</param>
        /// <param name="name">Display name.</param>
        /// <param name="isRtl">Whether the language is written right-to-left.</param>
        /// <returns>The created <see cref="Language"/> instance.</returns>
        /// <exception cref="ArgumentException">The code or name is missing or malformed.</exception>
        public static Language Create(string code, string name, bool isRtl)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(code);

            var normalizedCode = code.Trim().ToLowerInvariant();
            if (normalizedCode.Length > 10 || !CodePattern.IsMatch(normalizedCode))
                throw new ArgumentException("Language code must look like 'en', 'ar' or 'pt-br'.", nameof(code));

            return new Language { Code = normalizedCode, Name = NormalizeName(name), IsRtl = isRtl };
        }

        /// <summary>Changes the display name.</summary>
        public void Rename(string name) => Name = NormalizeName(name);

        /// <summary>Changes the writing direction.</summary>
        public void ChangeDirection(bool isRtl) => IsRtl = isRtl;

        private static string NormalizeName(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            var normalized = name.Trim();
            if (normalized.Length > 100)
                throw new ArgumentException("Language name cannot exceed 100 characters.", nameof(name));

            return normalized;
        }

        private static readonly Regex CodePattern =
            new("^[a-z]{2,3}(-[a-z0-9]{2,8})*$", RegexOptions.Compiled);
    }
}
