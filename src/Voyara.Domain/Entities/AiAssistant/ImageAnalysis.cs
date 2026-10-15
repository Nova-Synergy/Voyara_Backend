using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Entities.Identity;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.AiAssistant;

/// <summary>
/// Represents the result of computer vision and multimodal AI analysis performed on an uploaded tourist photograph or souvenir image.
/// </summary>
/// <remarks>
/// Detects physical objects, cultural artifacts, dishes, or landmarks and provides estimated local market price ranges and attribution metadata.
/// </remarks>
public sealed class ImageAnalysis : AuditableEntity
{
    /// <summary>
    /// Gets the URI pointing to the analyzed image in cloud storage.
    /// </summary>
    /// <value>The public or presigned URL of the source image.</value>
    public string ImageUrl { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the name or label of the primary landmark, artifact, or item detected by the model.
    /// </summary>
    /// <value>A descriptive text identifying the detected subject.</value>
    public string DetectedItem { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the estimated minimum purchase price or admission fee for the detected item.
    /// </summary>
    /// <value>The lower bound of the estimated price range.</value>
    public decimal MinPrice { get; private set; }

    /// <summary>
    /// Gets the estimated maximum fair price or admission fee for the detected item.
    /// </summary>
    /// <value>The upper bound of the estimated price range.</value>
    public decimal MaxPrice { get; private set; }

    /// <summary>
    /// Gets the three-letter ISO 4217 currency code for the price estimates.
    /// </summary>
    /// <value>The uppercase currency code (e.g., "EGP", "USD").</value>
    public string Currency { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the model's prediction confidence score between 0.0 and 1.0.
    /// </summary>
    /// <value>A floating-point value indicating classification confidence.</value>
    public double Confidence { get; private set; }

    /// <summary>
    /// Gets the serialized JSON payload representing visual feature factors and breakdown details.
    /// </summary>
    /// <value>Raw JSON string containing detection factors and visual attributes.</value>
    public string FactorsJson { get; private set; } = "{}";

    /// <summary>
    /// Gets the identifier of the user who requested the analysis.
    /// </summary>
    /// <value>The foreign key linking to the requesting <see cref="User"/>.</value>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Gets the navigation reference to the user who requested the analysis.
    /// </summary>
    /// <value>The owning <see cref="User"/> entity.</value>
    public User? User { get; private set; }

    /// <summary>
    /// Gets the identifier of the destination associated with the detected item, if mapped.
    /// </summary>
    /// <value>An optional foreign key to the matched <see cref="Destination"/>.</value>
    public Guid? DestinationId { get; private set; }

    /// <summary>
    /// Gets the navigation reference to the matched destination, if applicable.
    /// </summary>
    /// <value>The matched <see cref="Destination"/> entity, or <see langword="null"/>.</value>
    public Destination? Destination { get; private set; }

    private ImageAnalysis()
    {
    }

    private ImageAnalysis(
        Guid userId,
        string imageUrl,
        string detectedItem,
        decimal minPrice,
        decimal maxPrice,
        string currency,
        double confidence,
        string factorsJson,
        Guid? destinationId = null)
    {
        UserId = userId;
        ImageUrl = imageUrl;
        DetectedItem = detectedItem;
        MinPrice = Math.Max(0m, minPrice);
        MaxPrice = Math.Max(MinPrice, maxPrice);
        Currency = currency;
        Confidence = Math.Clamp(confidence, 0.0, 1.0);
        FactorsJson = string.IsNullOrWhiteSpace(factorsJson) ? "{}" : factorsJson;
        DestinationId = destinationId;
    }

    /// <summary>
    /// Factory method to construct a new <see cref="ImageAnalysis"/> entity.
    /// </summary>
    /// <param name="userId">The identifier of the user requesting analysis.</param>
    /// <param name="imageUrl">The cloud storage URL of the image.</param>
    /// <param name="detectedItem">The identified item, dish, or landmark.</param>
    /// <param name="minPrice">Minimum estimated price.</param>
    /// <param name="maxPrice">Maximum estimated price.</param>
    /// <param name="currency">ISO 4217 currency code.</param>
    /// <param name="confidence">Detection confidence between 0.0 and 1.0.</param>
    /// <param name="factorsJson">JSON representation of visual factors.</param>
    /// <param name="destinationId">Optional identifier of an associated catalog destination.</param>
    /// <returns>A new <see cref="ImageAnalysis"/> instance.</returns>
    public static ImageAnalysis Create(
        Guid userId,
        string imageUrl,
        string detectedItem,
        decimal minPrice,
        decimal maxPrice,
        string currency,
        double confidence,
        string factorsJson,
        Guid? destinationId = null)
    {
        return new ImageAnalysis(
            userId,
            imageUrl ?? string.Empty,
            detectedItem ?? string.Empty,
            minPrice,
            maxPrice,
            currency ?? string.Empty,
            confidence,
            factorsJson,
            destinationId);
    }
}
