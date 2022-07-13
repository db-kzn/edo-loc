namespace EDO_FOMS.Client.Infrastructure.Services
{
    public interface IClientState : IClientService
    {
        int Timezone { get; }
        int RowsPerPage { get; set; }
        bool MatchCase { get; set; }
        bool Dense { get; set; }
        bool FilterIsOpen { get; set; }

        int TooltipDelay { get; set; }
        int TooltipDuration { get; set; }

        //Task<IResult<NavCountsModel>> RefreshNavCountsAsync(string userId);
        //Task<IResult<Subscribe>> GetSubscribeAsync(string userId);
        //Task<IResult<int>> PostSubscribeAsync(Subscribe request);
    }
}
