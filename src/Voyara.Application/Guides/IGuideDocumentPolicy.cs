using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides;

/// <summary>Upload rules for verification documents; limits come from configuration.</summary>
public interface IGuideDocumentPolicy
{
    /// <summary>Maximum number of documents a profile may hold.</summary>
    int MaxDocumentsPerProfile { get; }

    /// <summary>Number of documents required before an application is complete (0 = not enforced).</summary>
    int RequiredDocumentCount { get; }

    /// <summary>
    /// Validates size, extension, declared content type and file signature.
    /// Returns the normalized lower-case extension (for example ".pdf") on success.
    /// </summary>
    Task<Result<string>> ValidateAsync(
        string fileName,
        string contentType,
        long length,
        Stream content,
        CancellationToken cancellationToken = default);
}
