using EDO_FOMS.Application.Responses.Identity;
using System.Collections.Generic;

namespace EDO_FOMS.Application.Requests.Identity
{
    public class UpdateUserRolesRequest
    {
        public string UserId { get; set; }
        public IList<UserRoleModel> UserRoles { get; set; }
    }
}