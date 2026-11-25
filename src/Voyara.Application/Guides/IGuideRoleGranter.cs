using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides
{
    /// <summary>
    /// Grants the TourGuide role. The implementation must work on the same unit of work as the caller
    /// and must not save, so that approval and role assignment commit together (SEQ-13).
    /// </summary>
    public interface IGuideRoleGranter
    {
        Task<Result> GrantTourGuideRoleAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
