using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyara.Application.DTOs.Guides
{
    /// <summary>Verification document metadata. Never includes storage names or paths.</summary>
    public sealed record GuideDocumentDto(
        Guid Id,
        string DocumentType,
        string OriginalFileName,
        string ContentType,
        long SizeBytes,
        DateTimeOffset? UploadedAt);
}
