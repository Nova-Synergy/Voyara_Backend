using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyara.Application.DTOs.Guides
{
    /// <summary>A document stream ready to be returned to an authorized caller.</summary>
    public sealed record GuideDocumentFile(Stream Content, string ContentType, string FileName);
}
