using EDO_FOMS.Application.Interfaces.Common;
using System.Collections.Generic;

namespace EDO_FOMS.Application.Interfaces.Services
{
    public interface ICurrentUserService : IService
    {
        string UserId { get; }
        //string Email { get; }
        //string GivenName { get; }
        List<KeyValuePair<string, string>> Claims { get; }
    }
}