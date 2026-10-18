namespace Voyara.Domain.Entities.Identity
{
    /// <summary>
    /// Represents an authorization role such as Tourist, TourGuide or Admin.
    /// </summary>
    public sealed class Role
    {
        private const int MaxNameLength = 50;

        /// <summary>
        /// Gets the database-generated identifier.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets the unique role name.
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        private Role()
        {
        }

        /// <summary>Creates a role.</summary>
        /// <param name="name">Unique role name, at most 50 characters.</param>
        /// <returns>The created <see cref="Role"/> instance.</returns>
        /// <exception cref="ArgumentException">The name is missing or too long.</exception>
        public static Role Create(string name) => new() { Name = NormalizeName(name) };

        /// <summary>Renames the role.</summary>
        public void Rename(string name) => Name = NormalizeName(name);

        private static string NormalizeName(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            var normalized = name.Trim();
            if (normalized.Length > MaxNameLength)
                throw new ArgumentException($"Name cannot exceed {MaxNameLength} characters.", nameof(name));

            return normalized;
        }
    }
}
