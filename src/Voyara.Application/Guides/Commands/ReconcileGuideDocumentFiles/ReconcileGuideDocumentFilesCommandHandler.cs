using MediatR;
using Microsoft.Extensions.Logging;
using Voyara.Application.Abstractions;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides.Commands.ReconcileGuideDocumentFiles;

public sealed class ReconcileGuideDocumentFilesCommandHandler
    : IRequestHandler<ReconcileGuideDocumentFilesCommand, Result<int>>
{
    private const int ChunkSize = 200;

    private readonly IFileStorage _fileStorage;
    private readonly IGuideDocumentFileCatalog _documents;
    private readonly IDateTime _dateTime;
    private readonly ILogger<ReconcileGuideDocumentFilesCommandHandler> _logger;

    public ReconcileGuideDocumentFilesCommandHandler(
        IFileStorage fileStorage,
        IGuideDocumentFileCatalog documents,
        IDateTime dateTime,
        ILogger<ReconcileGuideDocumentFilesCommandHandler> logger)
    {
        _fileStorage = fileStorage;
        _documents = documents;
        _dateTime = dateTime;
        _logger = logger;
    }

    public async Task<Result<int>> Handle(ReconcileGuideDocumentFilesCommand command, CancellationToken cancellationToken)
    {
        var cutoff = _dateTime.UtcNow - (command.GracePeriod ?? GuideDocumentStoragePolicy.OrphanGracePeriod);

        var candidates = (await _fileStorage.ListAsync(cancellationToken))
            .Where(f => f.Name.StartsWith(GuideDocumentStoragePolicy.FilePrefix, StringComparison.Ordinal)
                        && f.LastWriteUtc <= cutoff)
            .Select(f => f.Name)
            .ToList();

        var deleted = 0;
        foreach (var chunk in candidates.Chunk(ChunkSize))
        {
            var referenced = (await _documents.ListReferencedFileNamesAsync(chunk, cancellationToken)).ToHashSet();

            foreach (var name in chunk.Where(n => !referenced.Contains(n)))
            {
                try
                {
                    await _fileStorage.DeleteAsync(name, cancellationToken);
                    deleted++;
                }
                catch (Exception exception) when (exception is not OperationCanceledException)
                {
                    // Stays orphaned until the next run; one bad file must not stop the rest.
                    _logger.LogWarning(exception, "Could not delete orphaned guide document file {FileName}; will retry.", name);
                }
            }
        }

        if (deleted > 0)
        {
            _logger.LogInformation("Removed {Count} orphaned guide document files.", deleted);
        }

        return deleted;
    }
}
