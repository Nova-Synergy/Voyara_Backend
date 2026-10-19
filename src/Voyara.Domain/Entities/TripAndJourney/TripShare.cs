using System.Security.Cryptography;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.TripAndJourney;

/// <summary>
/// Represents a tokenized access share record granting collaborators or external users read access to a trip itinerary.
/// </summary>
public sealed class TripShare : BaseEntity
{
    /// <summary>
    /// Gets the identifier of the trip being shared.
    /// </summary>
    /// <value>The foreign key pointing to the target <see cref="Trip"/>.</value>
    public Guid TripId { get; private set; }

    /// <summary>
    /// Gets the secure, URL-safe random token used to authenticate share link requests.
    /// </summary>
    /// <value>A cryptographically random token string.</value>
    public string Token { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the optional UTC expiration timestamp after which the share token is no longer valid.
    /// </summary>
    /// <value>The expiration date and time, or <see langword="null"/> if the link never expires.</value>
    public DateTime? ExpiresAt { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the share link was manually revoked by the trip owner.
    /// </summary>
    /// <value>The revocation date and time, or <see langword="null"/> if the link remains unrevoked.</value>
    public DateTime? RevokedAt { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the share token is currently active and eligible for access.
    /// </summary>
    /// <value><see langword="true"/> if unrevoked and not expired; otherwise, <see langword="false"/>.</value>
    public bool IsActive => RevokedAt is null && (ExpiresAt is null || ExpiresAt > DateTime.UtcNow);

    private TripShare()
    {
    }

    private TripShare(Guid tripId, string token, DateTime? expiresAt)
    {
        TripId = tripId;
        Token = token;
        ExpiresAt = expiresAt;
    }

    /// <summary>
    /// Factory method to construct a new <see cref="TripShare"/> with a cryptographically secure token.
    /// </summary>
    /// <param name="tripId">The identifier of the trip to share.</param>
    /// <param name="expiresAt">Optional expiration timestamp in UTC.</param>
    /// <returns>A new <see cref="TripShare"/> instance.</returns>
    public static TripShare Create(Guid tripId, DateTime? expiresAt = null)
    {
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(16)).ToLowerInvariant();
        return new TripShare(tripId, token, expiresAt);
    }

    /// <summary>
    /// Revokes the share token, preventing any further access through its URL.
    /// </summary>
    public void Revoke()
    {
        if (RevokedAt is null)
        {
            RevokedAt = DateTime.UtcNow;
        }
    }
}

