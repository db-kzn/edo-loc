using EDO_FOMS.Domain.Contracts;
using EDO_FOMS.Domain.Enums;
using System;
using System.Collections.Generic;

namespace EDO_FOMS.Domain.Entities.Dir
{
    public class RouteStage : AuditableEntity<int>
    {
        public int RouteId { get; set; }                   // - Внешний индекс
        public virtual Route Route { get; set; }

        public int Number { get; set; }                    // + Номер этапа в цепочке маршрута

        public MudBlazor.Color Color { get; set; }         // + Цвет формы
        public string Name { get; set; }                   // + Наименование этапа
        public string Description { get; set; }            // - Описание этапа

        public ActTypes ActType { get; set; }              // - Тип этапа: неопределенный, подписание, согласование или рецензирование
        public bool InSeries { get; set; } = false;        // - Последовательное прохождение
        public bool AllRequred { get; set; } = true;       // - Если параллельно, то требуются все

        public bool DenyRevocation { get; set; }           // - Возможность отзывать документ с маршрута
        public TimeSpan Validity { get; set; }             // - Срок на прохождение этапа

        //public List<RouteStageStep> Steps { get; set; }    // + Процессы этапы
    }
}
