namespace EDO_FOMS.Client.Infrastructure.Routes
{
    public static class TokenEndpoints
    {
        public const string Get = "api/identity/token";
        public const string Refresh = "api/identity/token/refresh";
        public const string PostByCert = "api/identity/token/cert";
        public const string PostCheckCert = "api/identity/token/cert-check";
    }
}