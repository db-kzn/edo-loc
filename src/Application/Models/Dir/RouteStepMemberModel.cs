using EDO_FOMS.Application.Responses.Docums;
using EDO_FOMS.Domain.Enums;

namespace EDO_FOMS.Application.Models.Dir
{
    public class RouteStepMemberModel
    {
        // RouteStepId

        public ActTypes Act { get; set; } = ActTypes.Undefined; // Тип действия
        public bool IsAdditional { get; set; } = false;         // Дополнительный, не основной
        public string UserId { get; set; } = string.Empty;      // Участник

        public ContactResponse Contact { get; set; }            // Контакт участника
    }
}
