using EDO_FOMS.Domain.Enums;

namespace EDO_FOMS.Domain.Entities.Dir
{
    public class RouteStepMember
    {
        public int RouteStepId { get; set; }                    // Идентификатор процесса (шага)
        public RouteStep Step { get; set; }                     // Процесс, к которому относятся участники

        public ActTypes Act { get; set; } = ActTypes.Undefined; // Тип действия
        public bool IsAdditional { get; set; } = false;         // Дополнительный, не основной
        public string UserId { get; set; } = string.Empty;      // Участник
    }
}
