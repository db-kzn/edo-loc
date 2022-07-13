namespace EDO_FOMS.Client.Infrastructure.Routes
{
    public static class StateEndpoints
    {
        public const string Ctrl = "api/state/";

        public static string GetNavCounts(string userId) => $"{Ctrl}nav-counts/{userId}";
        public static string GetSubscribe(string userId) => $"{Ctrl}subscribe/{userId}";
        public static string PostSubscribe() => $"{Ctrl}subscribe";
    }
}
