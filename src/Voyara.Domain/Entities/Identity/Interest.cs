namespace Voyara.Domain.Entities.Identity
{
    /// <summary>
    /// Represents a topic a tourist can be interested in (history, food, beaches, ...).
    /// </summary>
    public sealed class Interest
    {
        private const int MaxCodeLength = 50;

        /// <summary>
        /// Gets the database-generated identifier.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets the unique code of the interest.
        /// </summary>
        public string Code { get; private set; } = string.Empty;

        private Interest()
        {
        }

        /// <summary>Creates an interest.</summary>
        /// <param name="code">Unique code, at most 50 characters.</param>
        /// <returns>The created <see cref="Interest"/> instance.</returns>
        /// <exception cref="ArgumentException">The code is missing or too long.</exception>
        public static Interest Create(string code)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(code);

            var normalized = code.Trim().ToLowerInvariant();
            if (normalized.Length > MaxCodeLength)
                throw new ArgumentException($"Code cannot exceed {MaxCodeLength} characters.", nameof(code));

            return new Interest { Code = normalized };
        }
    }
}
