namespace Voyara.Application.DTOs;

/// <summary>
/// Read model representing an individual itemized cost line.
/// </summary>
/// <param name="Description">Descriptive summary of the expense.</param>
/// <param name="Amount">Monetary cost amount.</param>
/// <param name="Currency">Three-letter ISO 4217 currency code.</param>
/// <param name="Category">The expense category name (e.g. Transportation, Accommodation, Activities).</param>
public sealed record CostLineResponse(
    string Description,
    decimal Amount,
    string Currency,
    string Category);

/// <summary>
/// Read model representing an estimated monetary range.
/// </summary>
public sealed record CostRangeResponse(
    decimal Minimum,
    decimal Maximum,
    string Currency);

/// <summary>
/// Read model representing a category-specific cost estimate with breakdown lines and range.
/// </summary>
public sealed record CategoryCostEstimateResponse(
    string Category,
    decimal TotalAmount,
    decimal MinAmount,
    decimal MaxAmount,
    string Currency,
    IReadOnlyList<CostLineResponse> Lines);

/// <summary>
/// Read model representing the overall cost estimate snapshot generated for a journey.
/// </summary>
public sealed record CostEstimateResultResponse(
    Guid Id,
    Guid JourneyId,
    Guid? TripId,
    Guid UserId,
    DateTimeOffset CalculatedAt,
    decimal TotalCost,
    decimal MinCost,
    decimal MaxCost,
    string Currency,
    IReadOnlyList<CategoryCostEstimateResponse> Categories,
    IReadOnlyList<CostLineResponse> Lines);

/// <summary>
/// Read model representing a complete journey cost breakdown including itemized lines, categories, range, and disclaimer.
/// </summary>
public sealed record JourneyCostBreakdownResponse(
    Guid JourneyId,
    Guid? EstimateId,
    DateTimeOffset? CalculatedAt,
    decimal TotalCost,
    decimal MinCost,
    decimal MaxCost,
    string Currency,
    IReadOnlyList<CategoryCostEstimateResponse> Categories,
    IReadOnlyList<CostLineResponse> Lines,
    string Disclaimer);

/// <summary>
/// Read model representing a currency-converted cost breakdown for a journey.
/// </summary>
public sealed record ConvertedCostBreakdownResponse(
    Guid JourneyId,
    Guid? EstimateId,
    string BaseCurrency,
    string TargetCurrency,
    decimal ExchangeRate,
    decimal ConvertedTotalCost,
    decimal ConvertedMinCost,
    decimal ConvertedMaxCost,
    IReadOnlyList<CategoryCostEstimateResponse> Categories,
    IReadOnlyList<CostLineResponse> Lines,
    string Disclaimer);
