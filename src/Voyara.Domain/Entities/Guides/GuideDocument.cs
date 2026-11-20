using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Guides
{
    /// <summary>
    /// Metadata of a verification document uploaded by a guide. The file itself lives in private storage;
    /// only the generated storage name is kept here and it is never exposed through the API.
    /// </summary>
    public sealed class GuideDocument : AuditableEntity
    {
        public const int MaxDocumentTypeLength = 100;
        public const int MaxOriginalFileNameLength = 255;
        public const int MaxStoredFileNameLength = 100;
        public const int MaxContentTypeLength = 100;

        /// <summary>Gets the identifier of the guide profile that owns the document.</summary>
        public Guid GuideProfileId { get; private set; }

        /// <summary>Gets the guide-provided description of the document kind.</summary>
        public string DocumentType { get; private set; } = string.Empty;

        /// <summary>Gets the sanitized original file name, used only for display and download.</summary>
        public string OriginalFileName { get; private set; } = string.Empty;

        /// <summary>Gets the server-generated name under which the file is stored.</summary>
        public string StoredFileName { get; private set; } = string.Empty;

        /// <summary>Gets the validated content type of the file.</summary>
        public string ContentType { get; private set; } = string.Empty;

        /// <summary>Gets the file size in bytes.</summary>
        public long SizeBytes { get; private set; }

        private GuideDocument()
        {
        }

        /// <summary>Creates the metadata record of an uploaded verification document.</summary>
        /// <exception cref="ArgumentException">Thrown when any value is missing or too long.</exception>
        public static GuideDocument Create(
            Guid guideProfileId,
            string documentType,
            string originalFileName,
            string storedFileName,
            string contentType,
            long sizeBytes)
        {
            if (guideProfileId == Guid.Empty)
                throw new ArgumentException("A guide profile is required.", nameof(guideProfileId));

            if (string.IsNullOrWhiteSpace(documentType) || documentType.Trim().Length > MaxDocumentTypeLength)
                throw new ArgumentException($"Document type is required (max {MaxDocumentTypeLength} characters).", nameof(documentType));

            if (string.IsNullOrWhiteSpace(originalFileName) || originalFileName.Trim().Length > MaxOriginalFileNameLength)
                throw new ArgumentException($"File name is required (max {MaxOriginalFileNameLength} characters).", nameof(originalFileName));

            if (string.IsNullOrWhiteSpace(storedFileName) || storedFileName.Length > MaxStoredFileNameLength)
                throw new ArgumentException("A stored file name is required.", nameof(storedFileName));

            if (string.IsNullOrWhiteSpace(contentType) || contentType.Trim().Length > MaxContentTypeLength)
                throw new ArgumentException("A content type is required.", nameof(contentType));

            if (sizeBytes <= 0)
                throw new ArgumentException("File size must be greater than zero.", nameof(sizeBytes));

            return new GuideDocument
            {
                GuideProfileId = guideProfileId,
                DocumentType = documentType.Trim(),
                OriginalFileName = originalFileName.Trim(),
                StoredFileName = storedFileName,
                ContentType = contentType.Trim().ToLowerInvariant(),
                SizeBytes = sizeBytes
            };
        }
    }
}
