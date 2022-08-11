using EDO_FOMS.Domain.Entities.Dir;
using EDO_FOMS.Domain.Enums;
using MudBlazor;
using System;
using System.Security.Cryptography;

namespace EDO_FOMS.Application.Models.Dir
{
    public class RouteStageModel
    {
        public int Id { get; set; }
        public int RouteId { get; set; }                   // - Внешний индекс
        //public virtual Route Route { get; set; }

        public int Number { get; set; }                    // + Номер этапа в цепочке маршрута

        public Color Color { get; set; }                   // + Цвет формы
        public string Name { get; set; }                   // + Наименование этапа
        public string Description { get; set; }            // - Описание этапа

        public ActTypes ActType { get; set; }              // - Тип этапа: неопределенный, подписание, согласование или рецензирование
        public bool InSeries { get; set; } = false;        // - Последовательное прохождение
        public bool AllRequred { get; set; } = true;       // - Если параллельно, то требуются все

        public bool DenyRevocation { get; set; }           // - Возможность отзывать документ с маршрута
        public TimeSpan Validity { get; set; }             // - Срок на прохождение этапа

        //public List<RouteStageStep> Steps { get; set; }    // + Процессы этапы

        public RouteStageModel() { }

        public RouteStageModel(RouteStage s)
        {
            Id = s.Id;
            RouteId = s.RouteId;
            Number = s.Number;

            Color = s.Color;
            Name = s.Name;
            Description = s.Description;

            ActType = s.ActType;
            InSeries = s.InSeries;
            AllRequred = s.AllRequred;

            DenyRevocation = s.DenyRevocation;
            Validity = s.Validity;
        }
    }
}
