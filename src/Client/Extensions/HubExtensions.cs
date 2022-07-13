using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using EDO_FOMS.Shared.Constants.Application;

namespace EDO_FOMS.Client.Extensions
{
    public static class HubExtensions
    {
        public static HubConnection TryInitialize(this HubConnection hubConnection, NavigationManager navigationManager)
        {
            if (hubConnection == null)
            {
                hubConnection = new HubConnectionBuilder()
                                  .WithUrl(navigationManager.ToAbsoluteUri(AppConstants.SignalR.HubUrl))
                                  .Build();
            }
            return hubConnection;
        }
    }
}