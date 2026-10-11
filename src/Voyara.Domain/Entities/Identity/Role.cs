namespace Voyara.Domain.Entities.Identity
{
    /// <summary>
    /// Represents an authorization role such as Tourist, TourGuide or Admin.
    /// </summary>
    public sealed class Role
    {
        /// <summary>
        /// Gets the database-generated identifier.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets the unique role name.
        /// </summary>
        public string Name { get; private set; } = string.Empty;
    }
}
