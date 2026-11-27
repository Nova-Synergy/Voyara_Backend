namespace Voyara.Application.Guides;

/// <summary>Naming and retention rules for guide document files in storage.</summary>
public static class GuideDocumentStoragePolicy
{
    /// <summary>Every stored guide document file name starts with this prefix; reconciliation never touches other files.</summary>
    public const string FilePrefix = "guide-doc-";

    /// <summary>
    /// A file without a live document row is only treated as an orphan after this age, so an upload whose database
    /// transaction has not committed yet is never removed.
    /// </summary>
    public static readonly TimeSpan OrphanGracePeriod = TimeSpan.FromHours(1);
}
