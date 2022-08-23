using EDO_FOMS.Application.Responses.Docums;
using EDO_FOMS.Domain.Enums;

namespace EDO_FOMS.Client.Models
{
    public class MemberModel
    {
        public string Label { get; set; } = string.Empty;

        public ActTypes Act { get; set; } = ActTypes.Undefined; // Тип действия
        public bool IsAdditional { get; set; } = false;         // Дополнительный, не основной
        public string UserId { get; set; } = string.Empty;      // Участник

        public ContactResponse Contact { get; set; }            // Контакт участника
    }
}
