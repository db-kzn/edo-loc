using EDO_FOMS.Shared.Wrapper;
using System.Threading.Tasks;
using EDO_FOMS.Application.Features.Dashboards.Queries.GetData;

namespace EDO_FOMS.Client.Infrastructure.Managers.Dashboard
{
    public interface IDashboardManager : IManager
    {
        Task<IResult<DashboardDataResponse>> GetDataAsync();
    }
}