using EDO_FOMS.Application.Models;
using EDO_FOMS.Client.Infrastructure.Extensions;
using EDO_FOMS.Domain.Entities.Public;
using EDO_FOMS.Shared.Wrapper;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Infrastructure.Managers.System
{
    public class StateManager : IStateManager
    {
        private readonly HttpClient _httpClient;
        public StateManager(HttpClient httpClient) {_httpClient = httpClient;}

        public async Task<IResult<NavCountsModel>> RefreshNavCountsAsync(string userId)
        {
            //if (string.IsNullOrWhiteSpace(userId)) { return null; }

            var response = await _httpClient.GetAsync(Routes.StateEndpoints.GetNavCounts(userId));
            return await response.ToResult<NavCountsModel>();

            //var result = await response.ToResult<NavCountsModel>();

            //if (!result.Succeeded) { return new NavCountsModel(); }

            ////var counts = result.Data;
            ////_state.NavCount.Progress = counts.Progress;

            //return result.Data;
        }

        public async Task<IResult<Subscribe>> GetSubscribeAsync(string userId)
        {
            var response = await _httpClient.GetAsync(Routes.StateEndpoints.GetSubscribe(userId));
            return await response.ToResult<Subscribe>();

            //if (!result.Succeeded) { return null; }
            //
            //return result.Data;
        }

        public async Task<IResult<int>> PostSubscribeAsync(Subscribe request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.StateEndpoints.PostSubscribe(), request);
            return await response.ToResult<int>();
        }
    }
}
