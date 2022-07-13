using EDO_FOMS.Application.Models.Chat;
using EDO_FOMS.Shared.Constants.Application;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using EDO_FOMS.Application.Interfaces.Chat;

namespace EDO_FOMS.Server.Hubs
{
    public class SignalRHub : Hub
    {
        public async Task OnConnectAsync(string userId)
        {
            await Clients.All.SendAsync(AppConstants.SignalR.ConnectUser, userId);
        }

        public async Task OnDisconnectAsync(string userId)
        {
            await Clients.All.SendAsync(AppConstants.SignalR.DisconnectUser, userId);
        }

        public async Task OnChangeRolePermissions(string userId, string roleId)
        {
            await Clients.All.SendAsync(AppConstants.SignalR.LogoutUsersByRole, userId, roleId);
        }

        public async Task SendMessageAsync(ChatHistory<IChatUser> chatHistory, string userName)
        {
            await Clients.All.SendAsync(AppConstants.SignalR.ReceiveMessage, chatHistory, userName);
        }

        public async Task ChatNotificationAsync(string message, string receiverUserId, string senderUserId)
        {
            await Clients.All.SendAsync(AppConstants.SignalR.ReceiveChatNotification, message, receiverUserId, senderUserId);
        }

        public async Task UpdateDashboardAsync()
        {
            await Clients.All.SendAsync(AppConstants.SignalR.ReceiveUpdateDashboard);
        }

        public async Task UpdateNavCountsAsync()
        {
            await Clients.All.SendAsync(AppConstants.SignalR.ReceiveUpdateNavCounts);
        }

        public async Task RegenerateTokensAsync()
        {
            await Clients.All.SendAsync(AppConstants.SignalR.ReceiveRegenerateTokens);
        }
    }
}