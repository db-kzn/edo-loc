using EDO_FOMS.Application.Responses.Identity;
using EDO_FOMS.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using EDO_FOMS.Application.Interfaces.Chat;
using EDO_FOMS.Application.Models.Chat;

namespace EDO_FOMS.Application.Interfaces.Services
{
    public interface IChatService
    {
        Task<Result<IEnumerable<ChatUserResponse>>> GetChatUsersAsync(string userId);

        Task<IResult> SaveMessageAsync(ChatHistory<IChatUser> message);

        Task<Result<IEnumerable<ChatHistoryResponse>>> GetChatHistoryAsync(string userId, string contactId);
    }
}