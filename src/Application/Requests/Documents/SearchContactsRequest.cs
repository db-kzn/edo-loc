using EDO_FOMS.Domain.Enums;

namespace EDO_FOMS.Application.Requests.Documents
{
    public class SearchContactsRequest
    {
        public OrgTypes OrgType { get; set; } = OrgTypes.Undefined;            // Тип организации
        public UserBaseRoles BaseRole { get; set; } = UserBaseRoles.Undefined; // Роль контактов
        public string SearchString { get; set; } = string.Empty;               // Строка поиска

        public int Take { get; set; } = 10;                                    // Максимум контаков в результате
        public int? OrgId { get; set; } = null;                                // Организация поиска
    }
}
