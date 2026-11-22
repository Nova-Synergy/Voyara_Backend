using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voyara.Application.DTOs.Guides
{
    /// <summary>A language spoken by a guide (request input).</summary>
    public sealed record GuideLanguageInput(string LanguageCode, string Proficiency);
}
