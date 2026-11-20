namespace Voyara.Infrastructure.FileStorage
{
    /// <summary>Configurable limits for guide verification documents (section "GuideDocuments").</summary>
    public sealed class GuideDocumentOptions
    {
        public const string SectionName = "GuideDocuments";

        /// <summary>Maximum file size in bytes. Default 5 MB.</summary>
        public long MaxFileSizeBytes { get; set; } = 5 * 1024 * 1024;

        /// <summary>Allowed lower-case extensions including the dot.</summary>
        public string[] AllowedExtensions { get; set; } = [".pdf", ".jpg", ".jpeg", ".png"];

        /// <summary>Maximum number of documents per profile.</summary>
        public int MaxDocumentsPerProfile { get; set; } = 10;

        /// <summary>Documents required for a complete application. The specification defines no minimum, so 0.</summary>
        public int RequiredDocumentCount { get; set; }
    }
}