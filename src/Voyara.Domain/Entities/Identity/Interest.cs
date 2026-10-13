namespace Voyara.Domain.Entities.Identity
{
    /// <summary>
    /// Represents a topic a tourist can be interested in (history, food, beaches, ...).
    /// </summary>
    public sealed class Interest
    {
        /// <summary>
        /// Gets the database-generated identifier.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets the unique code of the interest.
        /// </summary>
        public string Code { get; private set; } = string.Empty;
    }
}
