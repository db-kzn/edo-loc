using EDO_FOMS.Application.Models.Chat;
using EDO_FOMS.Application.Responses.Identity;
using EDO_FOMS.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using EDO_FOMS.Application.Interfaces.Chat;

namespace EDO_FOMS.Client.Infrastructure.Managers.Communication
{
    public interface IChatManager : IManager
    {
        Task<IResult<IEnumerable<ChatUserResponse>>> GetChatUsersAsync();

        Task<IResult> SaveMessageAsync(ChatHistory<IChatUser> chatHistory);

        Task<IResult<IEnumerable<ChatHistoryResponse>>> GetChatHistoryAsync(string cId);
    }
}