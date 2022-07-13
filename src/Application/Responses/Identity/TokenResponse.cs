using System;

namespace EDO_FOMS.Application.Responses.Identity
{
    public class TokenResponse
    {
        public string UserName { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string UserImageURL { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}