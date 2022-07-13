namespace EDO_FOMS.Shared.Constants.Application
{
    public static class AppConstants
    {
        public static class SignalR
        {
            public const string HubUrl = "/signalRHub";

            public const string SendUpdateNavCounts = "UpdateNavCountsAsync";
            public const string ReceiveUpdateNavCounts = "UpdateNavCounts";

            public const string SendUpdateDashboard = "UpdateDashboardAsync";
            public const string ReceiveUpdateDashboard = "UpdateDashboard";

            public const string SendRegenerateTokens = "RegenerateTokensAsync";
            public const string ReceiveRegenerateTokens = "RegenerateTokens";

            public const string ReceiveChatNotification = "ReceiveChatNotification";
            public const string SendChatNotification = "ChatNotificationAsync";

            public const string ReceiveMessage = "ReceiveMessage";
            public const string SendMessage = "SendMessageAsync";

            public const string OnConnect = "OnConnectAsync";
            public const string ConnectUser = "ConnectUser";

            public const string OnDisconnect = "OnDisconnectAsync";
            public const string DisconnectUser = "DisconnectUser";

            public const string OnChangeRolePermissions = "OnChangeRolePermissions";
            public const string LogoutUsersByRole = "LogoutUsersByRole";
        }

        public static class Cache
        {
            public const string GetAllOrgsCacheKey = "all-orgs";
            public const string SubscribeCacheKey = "subscribe";
            public const string GetAllCertsCacheKey = "all-certs";
            public const string GetAllDocsCacheKey = "all-docs";
            public const string GetAllAgreementsCacheKey = "all-agreements";
            public const string GetAllDocumentTypesCacheKey = "all-document-types";

            public static string GetAllEntityExtendedAttributesCacheKey(string entityFullName)
            {
                return $"all-{entityFullName}-extended-attributes";
            }

            public static string GetAllEntityExtendedAttributesByEntityIdCacheKey<TEntityId>(string entityFullName, TEntityId entityId)
            {
                return $"all-{entityFullName}-extended-attributes-{entityId}";
            }
        }

        public static class MimeTypes
        {
            public const string OpenXml = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        }
    }
}