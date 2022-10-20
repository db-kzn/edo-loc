using EDO_FOMS.Application.Responses.Agreements;
using EDO_FOMS.Client.Infrastructure.Managers.Doc.Document;
using EDO_FOMS.Client.Infrastructure.Model.Docs;
using EDO_FOMS.Client.Models;
using EDO_FOMS.Domain.Enums;
using LinqKit;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Pages
{
    public partial class DocAgreementsDialog
    {
        [Inject] private IDocumentManager DocManager { get; set; }

        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        [Parameter] public DocModel Doc { get; set; }

        public DocAgrsCardModel DocAgrsCard { get; set; } = new();
        private MudTable<DocAgrsCardStageModel> _mudTable;
        public DocAgrsCardStageModel _stage;

        //private Dictionary<int, DocAgrsCardStageModel> DocCardStages = new();

        //private MudTable<KeyValuePair<int, DocAgrsCardStageModel>> _mudTable;
        //private KeyValuePair<int, DocAgrsCardStageModel> _stage;

        private bool _loaded = false;
        private bool _isAnswered = false;
        private bool _isMain = false;

        private string _action = "";

        private int tz;
        private int delay;
        private int duration;

        protected override async Task OnInitializedAsync()
        {
            tz = _stateService.Timezone;
            delay = _stateService.TooltipDelay;
            duration = _stateService.TooltipDuration;

            await GetDocAgrsCardAsync();

            _loaded = true;
        }

        private async Task GetDocAgrsCardAsync()
        {
            var response = await DocManager.GetDocAgrsCardAsync(Doc.DocId, Doc.AgreementId);

            if (!response.Succeeded)
            {
                response.Messages.ForEach((m) => _snackBar.Add(m, Severity.Error));
                return;
            }

            var card = response.Data;

            await _jsRuntime.InvokeVoidAsync("azino.Console", card, "DOC AGRS CARD: ");

            DocAgrsCard.DocId = card.DocId;
            DocAgrsCard.RouteId = card.RouteId;

            DocAgrsCard.EmplId = card.EmplId;
            DocAgrsCard.EmplOrgId = card.EmplOrgId;
            DocAgrsCard.ExecutorId = card.ExecutorId;

            DocAgrsCard.Number = card.Number;
            DocAgrsCard.Date = card.Date;
            DocAgrsCard.Title = card.Title;
            DocAgrsCard.Description = card.Description;

            card.Agreements.ForEach(a =>
            {
                if (DocAgrsCard.Stages.TryGetValue(a.StageNumber, out DocAgrsCardStageModel stage))
                {
                    stage.Agreements.Add(new DocAgrsCardAgreementModel(a));
                }
                else
                {
                    DocAgrsCard.Stages.Add(a.StageNumber, new DocAgrsCardStageModel(a));
                }
            });

            //var stages = DocAgrsCard.Stages.Values;
        }

        private void OnStageClick() => ShowBtnPress(_stage);
        private void ShowBtnPress(DocAgrsCardStageModel stage)
        {
            if (stage != null)
            {
                stage.ShowAgreements = !stage.ShowAgreements;
            }
        }

        private void ClickOk() => MudDialog.Close(DialogResult.Cancel());
        private void ClickAgree()
        {
            var state = (_action == nameof(ActTypes.Agreement)) ?
                nameof(AgreementActions.ToApprove) :
                (_action == nameof(ActTypes.Signing)) ?
                nameof(AgreementActions.ToSign) :
                nameof(AgreementActions.ToReview);

            MudDialog.Close(DialogResult.Ok(state));
        }
        private void ClickReject()
        {
            var state = (_action == nameof(ActTypes.Signing) || _action == nameof(ActTypes.Agreement))
                ? nameof(AgreementActions.ToReject)
                : nameof(AgreementActions.ToRefuse);

            MudDialog.Close(DialogResult.Ok(state));
        }
        private void ClickAddMembers() => MudDialog.Close(DialogResult.Ok(nameof(AgreementActions.AddMembers)));

        private Func<DocAgrsCardStageModel, int, string> RowStyle => (a, _) => "";
        //    (a.UserOrgId == Doc.KeyOrgId) ? " font-style: italic;" : "";
    }
}
