namespace Voyara.Application.Abstractions;

/// <summary>Minimal binary file storage contract.</summary>
public interface IFileStorage
{
    /// <summary>Saves a stream and returns the stored reference key.</summary>
    Task<string> SaveAsync(string fileName, Stream content, CancellationToken cancellationToken = default);

    /// <summary>Opens a stored file for reading; null when not found.</summary>
    Task<Stream?> OpenReadAsync(string fileName, CancellationToken cancellationToken = default);

    /// <summary>Deletes a stored file.</summary>
    Task DeleteAsync(string fileName, CancellationToken cancellationToken = default);
}
