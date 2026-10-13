using Microsoft.Extensions.Options;
using Voyara.Application.Abstractions;

namespace Voyara.Infrastructure.FileStorage;

/// <summary>Minimal local-filesystem implementation of <see cref="IFileStorage"/>; a stub for a real object store.</summary>
public sealed class LocalFileStorage : IFileStorage
{
    private readonly string _rootPath;

    /// <summary>Creates the storage with its configured root path.</summary>
    public LocalFileStorage(IOptions<FileStorageOptions> options) => _rootPath = options.Value.RootPath;

    /// <inheritdoc />
    public async Task<string> SaveAsync(string fileName, Stream content, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(_rootPath);
        var safeName = Path.GetFileName(fileName);
        var fullPath = Path.Combine(_rootPath, safeName);

        await using var target = File.Create(fullPath);
        await content.CopyToAsync(target, cancellationToken);

        return safeName;
    }

    /// <inheritdoc />
    public Task<Stream?> OpenReadAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_rootPath, Path.GetFileName(fileName));
        return Task.FromResult<Stream?>(File.Exists(fullPath) ? File.OpenRead(fullPath) : null);
    }

    /// <inheritdoc />
    public Task DeleteAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_rootPath, Path.GetFileName(fileName));
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }
}
