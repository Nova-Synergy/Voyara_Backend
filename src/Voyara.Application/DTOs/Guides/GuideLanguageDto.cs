using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyara.Application.DTOs.Guides
{
    /// <summary>A language spoken by a guide (response DTO).</summary>
    public sealed record GuideLanguageDto(string LanguageCode, string Proficiency);
}
