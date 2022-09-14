using EDO_FOMS.Application.Interfaces.Common;
using EDO_FOMS.Application.Responses.Audit;
using EDO_FOMS.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Interfaces.Services
{
    public interface IAuditService : IService
    {
        Task<IResult<IEnumerable<AuditResponse>>> GetCurrentUserTrailsAsync(string userId);

        Task<IResult<string>> ExportToExcelAsync(string userId, string searchString = "", bool searchInOldValues = false, bool searchInNewValues = false);
    }
}