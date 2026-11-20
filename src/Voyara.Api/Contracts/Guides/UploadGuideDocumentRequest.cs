namespace Voyara.Api.Contracts.Guides
{
    /// <summary>Multipart form of a verification document upload.</summary>
    public sealed class UploadGuideDocumentRequest
    {
        public string DocumentType { get; set; } = string.Empty;

        public IFormFile? File { get; set; }
    }
}
