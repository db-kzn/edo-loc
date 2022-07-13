using EDO_FOMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace EDO_FOMS.Server.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            Claims = httpContextAccessor.HttpContext?.User?.Claims.AsEnumerable().Select(item => new KeyValuePair<string, string>(item.Type, item.Value)).ToList();

            //Email = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
            //GivenName = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.GivenName);
        }

        public string UserId { get; }
        public List<KeyValuePair<string, string>> Claims { get; }

        //public string Email { get; }
        //public string GivenName { get; }
        //public bool EmailConfirmed { get; }
    }
}