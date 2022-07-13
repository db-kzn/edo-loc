using EDO_FOMS.Application.Models;
using EDO_FOMS.Domain.Entities.Public;
using EDO_FOMS.Shared.Wrapper;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Infrastructure.Managers.System
{
    public interface IStateManager : IManager
    {
        Task<IResult<NavCountsModel>> RefreshNavCountsAsync(string userId);
        Task<IResult<Subscribe>> GetSubscribeAsync(string userId);
        Task<IResult<int>> PostSubscribeAsync(Subscribe request);
    }
}
