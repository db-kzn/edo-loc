using AutoMapper;
using EDO_FOMS.Application.Exceptions;
using EDO_FOMS.Application.Interfaces.Services;
using EDO_FOMS.Application.Interfaces.Services.Identity;
using EDO_FOMS.Application.Models.Chat;
using EDO_FOMS.Application.Responses.Identity;
using EDO_FOMS.Infrastructure.Contexts;
using EDO_FOMS.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDO_FOMS.Application.Interfaces.Chat;
using EDO_FOMS.Infrastructure.Models.Identity;
using EDO_FOMS.Shared.Constants.Role;
using Microsoft.Extensions.Localization;

namespace EDO_FOMS.Infrastructure.Services
{
    public class ChatService : IChatService
    {
        private readonly EdoFomsContext _context;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IStringLocalizer<ChatService> _localizer;

        public ChatService(
            EdoFomsContext context,
            IMapper mapper,
            IUserService userService,
            IStringLocalizer<ChatService> localizer)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
            _localizer = localizer;
        }

        public async Task<Result<IEnumerable<ChatHistoryResponse>>> GetChatHistoryAsync(string userId, string contactId)
        {
            var response = await _userService.GetAsync(userId);
            if (response.Succeeded)
            {
                var user = response.Data;
                var query = await _context.ChatHistories
                    .Where(h => (h.FromUserId == userId && h.ToUserId == contactId) || (h.FromUserId == contactId && h.ToUserId == userId))
                    .OrderBy(a => a.CreatedDate)
                    .Include(a => a.FromUser)
                    .Include(a => a.ToUser)
                    .Select(x => new ChatHistoryResponse
                    {
                        FromUserId = x.FromUserId,
                        FromUserFullName = $"{x.FromUser.UserName}",
                        Message = x.Message,
                        CreatedDate = x.CreatedDate,
                        Id = x.Id,
                        ToUserId = x.ToUserId,
                        ToUserFullName = $"{x.ToUser.UserName}",
                        ToUserImageURL = x.ToUser.ProfilePictureDataUrl,
                        FromUserImageURL = x.FromUser.ProfilePictureDataUrl
                    }).ToListAsync();
                return await Result<IEnumerable<ChatHistoryResponse>>.SuccessAsync(query);
            }
            else
            {
                throw new ApiException(_localizer["User Not Found!"]);
            }
        }

        public async Task<Result<IEnumerable<ChatUserResponse>>> GetChatUsersAsync(string userId)
        {
            var userRoles = await _userService.GetRolesAsync(userId);
            var userIsAdmin = userRoles.Data?.UserRoles?.Any(x => x.Selected && x.RoleName == RoleConstants.Admin) == true;
            var allUsers = await _context.Users.Where(user => user.Id != userId && (userIsAdmin || user.IsActive && user.EmailConfirmed)).ToListAsync();
            var chatUsers = _mapper.Map<IEnumerable<ChatUserResponse>>(allUsers);
            return await Result<IEnumerable<ChatUserResponse>>.SuccessAsync(chatUsers);
        }

        public async Task<IResult> SaveMessageAsync(ChatHistory<IChatUser> message)
        {
            message.ToUser = await _context.Users.Where(user => user.Id == message.ToUserId).FirstOrDefaultAsync();
            await _context.ChatHistories.AddAsync(_mapper.Map<ChatHistory<EdoFomsUser>>(message));
            await _context.SaveChangesAsync();
            return await Result.SuccessAsync();
        }
    }
}