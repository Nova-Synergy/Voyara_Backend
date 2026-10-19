using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Catalog
{
    /// <summary>
    /// Represents a classification of destinations (museum, beach, restaurant, ...).
    /// </summary>
    public sealed class Category : BaseEntity
    {
        private const int MaxCodeLength = 50;
        private const int MaxIconLength = 200;

        /// <summary>
        /// Gets the unique code of the category.
        /// </summary>
        public string Code { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the icon name or URL shown with the category.
        /// </summary>
        public string Icon { get; private set; } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether the category is visible to tourists.
        /// </summary>
        public bool IsActive { get; private set; }

        private Category()
        {
        }

        /// <summary>Creates a category. It stays hidden from tourists until <see cref="Activate"/> is called.</summary>
        /// <param name="code">Unique code, at most 50 characters.</param>
        /// <param name="icon">Icon name or URL, at most 200 characters.</param>
        /// <returns>The created <see cref="Category"/> instance.</returns>
        /// <exception cref="ArgumentException">The code or icon is missing or too long.</exception>
        public static Category Create(string code, string icon)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(code);

            var normalizedCode = code.Trim().ToLowerInvariant();
            if (normalizedCode.Length > MaxCodeLength)
                throw new ArgumentException($"Code cannot exceed {MaxCodeLength} characters.", nameof(code));

            return new Category { Code = normalizedCode, Icon = NormalizeIcon(icon) };
        }

        /// <summary>Changes the icon.</summary>
        public void ChangeIcon(string icon) => Icon = NormalizeIcon(icon);

        /// <summary>Makes the category visible to tourists.</summary>
        /// <exception cref="InvalidOperationException">The category is already active.</exception>
        public void Activate()
        {
            if (IsActive)
                throw new InvalidOperationException("The category is already active.");

            IsActive = true;
        }

        /// <summary>Hides the category from tourists.</summary>
        /// <exception cref="InvalidOperationException">The category is already inactive.</exception>
        public void Deactivate()
        {
            if (!IsActive)
                throw new InvalidOperationException("The category is already inactive.");

            IsActive = false;
        }

        private static string NormalizeIcon(string icon)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(icon);

            var normalized = icon.Trim();
            if (normalized.Length > MaxIconLength)
                throw new ArgumentException($"Icon cannot exceed {MaxIconLength} characters.", nameof(icon));

            return normalized;
        }
    }
}