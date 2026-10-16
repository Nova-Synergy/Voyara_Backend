using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara_DomainModel.Common;

namespace Voyara_DomainModel.Community.Reviews
{
    /// <summary>
    /// A photo attached to a <see cref="Review"/>. It only exists as part of a review,
    /// so it can only be created through the review (the factory is <c>internal</c>).
    /// </summary>
    public class ReviewPhoto : Entity<Guid>
    {
        public Guid Id { get; private set; }

        public Guid ReviewId { get; private set; }

        /// <summary>Location of the stored image.</summary>
        public string Url { get; private set; } = default!;

        private ReviewPhoto()
        {
        }

        /// <summary>Creates a photo from an image URL.</summary>
        /// <param name="url">The location of the image.</param>
        /// <exception cref="DomainException">Thrown when the URL is empty.</exception>
        internal static ReviewPhoto Create(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new DomainException("Photo URL is required.");

            return new ReviewPhoto { Id = Guid.NewGuid(), Url = url.Trim() };
        }
    }
}

