using Voyara.Application.Abstractions.Messaging;

namespace Voyara.Application.Guides.Commands.ReconcileGuideDocumentFiles;

/// <summary>
/// Deletes stored guide document files that no live document refers to (failed uploads, failed deletions) and are older
/// than the grace period. Returns the number of files removed. Invoked by the background worker only.
/// </summary>
public sealed record ReconcileGuideDocumentFilesCommand(TimeSpan? GracePeriod = null) : ICommand<int>;
