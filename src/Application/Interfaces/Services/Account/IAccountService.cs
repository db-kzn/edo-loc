using EDO_FOMS.Application.Interfaces.Common;
using EDO_FOMS.Application.Requests.Identity;
using EDO_FOMS.Shared.Wrapper;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Interfaces.Services.Account
{
    public interface IAccountService : IService
    {
        Task<IResult> UpdateProfileAsync(UpdateProfileRequest model, string userId);

        Task<IResult> ChangePasswordAsync(ChangePasswordRequest model, string userId);

        Task<IResult<string>> GetProfilePictureAsync(string userId);

        Task<IResult<string>> UpdateProfilePictureAsync(UpdateProfilePictureRequest request, string userId);
    }
}