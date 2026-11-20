using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.Application.Guides;
using Voyara.SharedKernel.Results;

namespace Voyara.Infrastructure.FileStorage
{
    /// <summary>
    /// Validates verification documents by extension, declared content type, size and file signature,
    /// so a renamed executable cannot be stored as a "PDF".
    /// </summary>
    public sealed class GuideDocumentPolicy : IGuideDocumentPolicy
    {
        private static readonly Dictionary<string, (string ContentType, byte[][] Signatures)> Formats =
            new(StringComparer.OrdinalIgnoreCase)
            {
                [".pdf"] = ("application/pdf", new[] { new byte[] { 0x25, 0x50, 0x44, 0x46 } }),
                [".jpg"] = ("image/jpeg", new[] { new byte[] { 0xFF, 0xD8, 0xFF } }),
                [".jpeg"] = ("image/jpeg", new[] { new byte[] { 0xFF, 0xD8, 0xFF } }),
                [".png"] = ("image/png", new[] { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } })
            };

        private readonly GuideDocumentOptions _options;

        public GuideDocumentPolicy(IOptions<GuideDocumentOptions> options) => _options = options.Value;

        public int MaxDocumentsPerProfile => _options.MaxDocumentsPerProfile;

        public int RequiredDocumentCount => _options.RequiredDocumentCount;

        public async Task<Result<string>> ValidateAsync(
            string fileName,
            string contentType,
            long length,
            Stream content,
            CancellationToken cancellationToken = default)
        {
            if (length <= 0)
                return Result.Failure<string>(Invalid("The file is empty."));

            if (length > _options.MaxFileSizeBytes)
                return Result.Failure<string>(Invalid($"The file exceeds the maximum size of {_options.MaxFileSizeBytes} bytes."));

            var extension = Path.GetExtension(fileName ?? string.Empty).ToLowerInvariant();
            var allowed = _options.AllowedExtensions.Select(e => e.ToLowerInvariant()).ToHashSet();
            if (!allowed.Contains(extension) || !Formats.TryGetValue(extension, out var format))
                return Result.Failure<string>(Invalid("This file type is not allowed."));

            var declared = (contentType ?? string.Empty).Split(';')[0].Trim();
            if (!string.Equals(declared, format.ContentType, StringComparison.OrdinalIgnoreCase))
                return Result.Failure<string>(Invalid("The content type does not match the file extension."));

            if (!content.CanSeek)
                return Result.Failure<string>(Invalid("The file stream cannot be verified."));

            var header = new byte[8];
            content.Position = 0;
            var read = await content.ReadAsync(header.AsMemory(0, header.Length), cancellationToken);
            content.Position = 0;

            var matchesSignature = format.Signatures.Any(sig =>
                read >= sig.Length && header.AsSpan(0, sig.Length).SequenceEqual(sig));
            if (!matchesSignature)
                return Result.Failure<string>(Invalid("The file content does not match its type."));

            return Result.Success(extension);
        }

        private static Error Invalid(string description) => Voyara.Application.Guides.GuideErrors.InvalidDocument(description);
    }

}
