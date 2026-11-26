namespace Voyara.Application.Guides;

/// <summary>Read-only lookup used to reconcile stored guide document files with the database.</summary>
public interface IGuideDocumentFileCatalog
{
    /// <summary>Returns the subset of the given stored file names that belong to a live (not deleted) document.</summary>
    Task<IReadOnlyCollection<string>> ListReferencedFileNamesAsync(
        IReadOnlyCollection<string> storedFileNames, CancellationToken cancellationToken = default);
}
