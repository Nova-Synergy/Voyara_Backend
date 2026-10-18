namespace Voyara.Domain.ValueObjects;

/// <summary>
/// Encapsulates the outcome of validating a domain model or aggregate invariant.
/// </summary>
/// <remarks>
/// Contains a boolean flag indicating overall validity and an immutable collection of error
/// descriptions when validation rules fail.
/// </remarks>
public sealed class ValidationResult
{
    private static readonly ValidationResult ValidInstance = new(true, Array.Empty<string>());

    /// <summary>
    /// Gets a value indicating whether the validated entity conforms to all business rules.
    /// </summary>
    /// <value><see langword="true"/> if no validation errors were encountered; otherwise, <see langword="false"/>.</value>
    public bool IsValid { get; }

    /// <summary>
    /// Gets the list of validation error descriptions detailing violated invariants.
    /// </summary>
    /// <value>A read-only collection of error strings, empty if validation succeeded.</value>
    public IReadOnlyList<string> Errors { get; }

    private ValidationResult(bool isValid, IReadOnlyList<string> errors)
    {
        IsValid = isValid;
        Errors = errors;
    }

    /// <summary>
    /// Creates a successful validation result with no errors.
    /// </summary>
    /// <returns>A successful <see cref="ValidationResult"/> instance.</returns>
    public static ValidationResult Success() => ValidInstance;

    /// <summary>
    /// Creates a failed validation result containing the specified collection of error messages.
    /// </summary>
    /// <param name="errors">The collection of validation error descriptions.</param>
    /// <returns>A failed <see cref="ValidationResult"/> instance containing the reported errors.</returns>
    public static ValidationResult Failure(IEnumerable<string> errors)
    {
        var errorList = errors?.ToList() ?? new List<string>();
        return new ValidationResult(false, errorList.AsReadOnly());
    }

    /// <summary>
    /// Creates a failed validation result containing one or more error messages.
    /// </summary>
    /// <param name="errors">One or more validation error descriptions.</param>
    /// <returns>A failed <see cref="ValidationResult"/> instance containing the reported errors.</returns>
    public static ValidationResult Failure(params string[] errors) =>
        new(false, (errors ?? Array.Empty<string>()).ToList().AsReadOnly());
}

