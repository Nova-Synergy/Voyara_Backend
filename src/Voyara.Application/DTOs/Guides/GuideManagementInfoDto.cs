using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyara.Application.DTOs.Guides
{
    /// <summary>Information visible only to the profile owner and administrators.</summary>
    public sealed record GuideManagementInfoDto(
        string? RejectionReason,
        IReadOnlyList<GuideDocumentDto> Documents);
}
