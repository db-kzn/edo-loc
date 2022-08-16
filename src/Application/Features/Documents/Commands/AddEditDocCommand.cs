using EDO_FOMS.Application.Features.Documents.Commands.AddEdit;
using EDO_FOMS.Application.Requests;
using EDO_FOMS.Application.Serialization.JsonConverters;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EDO_FOMS.Application.Features.Documents.Commands;

public partial class AddEditDocCommand : IRequest<Result<int>>
{
    public int Id { get; set; } = 0;                                           // 0 - Новый документ
    public int? ParentId { get; set; }                                         // Родительский документ

    public string EmplId { get; set; } = string.Empty;                         // Инициатор подписания
    public int EmplOrgId { get; set; } = 0;                                    // Организация инициатора

    public int TypeId { get; set; } = 1;                                       // Тип документа: 1 - Договор, 2 - Приложение 
    [JsonConverter(typeof(TrimmingJsonConverter))]
    public string Number { get; set; } = string.Empty;                         // Номер
    public DateTime? Date { get; set; } = DateTime.Today;                      // Дата

    [JsonConverter(typeof(TrimmingJsonConverter))]
    public string Title { get; set; } = "";                                    // Наименование
    [JsonConverter(typeof(TrimmingJsonConverter))]
    public string Description { get; set; } = "";                              // Описание
    public bool IsPublic { get; set; } = false;                                // Публичный документ - виден всем сотрудникам Организации

    public int RouteId { get; set; } = 0;                                      // Маршрут документа. 0 - не определен
    public DocStages Stage { get; set; } = DocStages.Undefined;                // Статус документа:
    public int CurrentStep { get; set; } = 0;                                  // Текущий этап подписания
    public int TotalSteps { get; set; } = 0;                                   // Всего этапов в маршруте

    public string URL { get; set; } = string.Empty;                            // Ссылка для загрузки
    public UploadRequest UploadRequest { get; set; }                           // Сервис загруки файла

    public List<ActMember> Members { get; set; }
}

public class ActMember
{
    public int? StepId { get; set; }                                           // Идентификатор процесса маршрута, к которому относится участник
    public bool Additional { get; set; } = false;                              // Дополнительный участник, не основной

    public AgreementActions Action { get; set; } = AgreementActions.Undefined; // Тип процесса: подписание, согласование или рецензирование
    public OrgTypes OrgType { get; set; } = OrgTypes.Undefined;                // Тип организации, может быть не определен
    public bool OnlyHead { get; set; } = true;                                 // Требуется руководитель

    public string OrgInn { get; set; } = string.Empty;                         // ИНН Организации, если не зарегистрирована в системе
    public int? OrgId { get; set; } = null;                                    // ИД зарегистрированной Организации
    public string EmplId { get; set; } = string.Empty;                         // ИД сотрудника, если определен
}
