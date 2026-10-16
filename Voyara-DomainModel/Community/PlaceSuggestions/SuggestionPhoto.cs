using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara_DomainModel.Common;

namespace Voyara_DomainModel.Community.PlaceSuggestions
{
    /// <summary>
    /// A photo attached to a <see cref="PlaceSuggestion"/>. It only exists as part of
    /// a suggestion, so it is created through the suggestion (the factory is <c>internal</c>).
    /// </summary>
    public sealed class SuggestionPhoto : Entity<Guid>
    {

        public Guid PlaceSuggestionId { get; private set; }

        /// <summary>Location of the stored image.</summary>
        public string Url { get; private set; } = default!;

        private SuggestionPhoto() { }

        /// <summary>Creates a photo from an image URL.</summary>
        /// <param name="url">The location of the image.</param>
        /// <exception cref="DomainException">Thrown when the URL is empty.</exception>
        internal static SuggestionPhoto Create(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new DomainException("Photo URL is required.");

            return new SuggestionPhoto { Id = Guid.NewGuid(), Url = url.Trim() };
        }
    }
}
