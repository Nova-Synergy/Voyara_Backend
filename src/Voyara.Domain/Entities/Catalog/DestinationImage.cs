using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Catalog
{
    /// <summary>
    /// Represents a picture of a destination. It is created and changed only through its <see cref="Destination"/>.
    /// </summary>
    public sealed class DestinationImage : BaseEntity
    {
        internal const int MaxUrlLength = 2048;

        /// <summary>
        /// Gets the location of the image file.
        /// </summary>
        public string Url { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the position of the image in the gallery.
        /// </summary>
        public int SortOrder { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this is the cover image.
        /// </summary>
        public bool IsCover { get; private set; }

        /// <summary>
        /// Gets the identifier of the owning destination.
        /// </summary>
        public Guid DestinationId { get; private set; }

        /// <summary>
        /// Gets the destination that owns this image.
        /// </summary>
        public Destination Destination { get; private set; } = null!;

        private DestinationImage()
        {
        }

        internal static DestinationImage Create(Destination destination, string url, int sortOrder, bool isCover)
        {
            ArgumentNullException.ThrowIfNull(destination);
            ArgumentException.ThrowIfNullOrWhiteSpace(url);

            var normalizedUrl = url.Trim();
            if (normalizedUrl.Length > MaxUrlLength)
                throw new ArgumentException($"Url cannot exceed {MaxUrlLength} characters.", nameof(url));

            return new DestinationImage
            {
                Destination = destination,
                DestinationId = destination.Id,
                Url = normalizedUrl,
                SortOrder = sortOrder,
                IsCover = isCover
            };
        }

        internal void MarkAsCover(bool isCover) => IsCover = isCover;

        internal void MoveTo(int sortOrder) => SortOrder = sortOrder;
    }
}
