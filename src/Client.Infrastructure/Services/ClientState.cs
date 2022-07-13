namespace EDO_FOMS.Client.Infrastructure.Services
{
    public class ClientState : IClientState
    {
        //private readonly HttpClient _httpClient;
        //public StateService(HttpClient httpClient) { _httpClient = httpClient; }

        // + State of: Debug, Prod, etc

        public int Timezone { get; } = +3;
        public int RowsPerPage { get; set; } = 10;
        public bool MatchCase { get; set; } = false;
        public bool Dense { get; set; } = true;
        public bool FilterIsOpen { get; set; } = false;

        public int TooltipDelay { get; set; } = 1000;
        public int TooltipDuration { get; set; } = 500;

        //public async Task<IResult<NavCountsModel>> RefreshNavCountsAsync(string userId)
        //{
        //    //if (string.IsNullOrWhiteSpace(userId)) { return null; }

        //    var response = await _httpClient.GetAsync(Routes.StateEndpoints.GetNavCounts(userId));
        //    return await response.ToResult<NavCountsModel>();

        //    //var result = await response.ToResult<NavCountsModel>();

        //    //if (!result.Succeeded) { return new NavCountsModel(); }

        //    ////var counts = result.Data;
        //    ////_state.NavCount.Progress = counts.Progress;

        //    //return result.Data;
        //}

        //public async Task<IResult<Subscribe>> GetSubscribeAsync(string userId)
        //{
        //    var response = await _httpClient.GetAsync(Routes.StateEndpoints.GetSubscribe(userId));
        //    return await response.ToResult<Subscribe>();

        //    //if (!result.Succeeded) { return null; }
        //    //
        //    //return result.Data;
        //}

        //public async Task<IResult<int>> PostSubscribeAsync(Subscribe request)
        //{
        //    var response = await _httpClient.PostAsJsonAsync(Routes.StateEndpoints.PostSubscribe(), request);
        //    return await response.ToResult<int>();
        //}
    }
}
