using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using EDO_FOMS.Client.Infrastructure.Managers.Doc.Document;
using EDO_FOMS.Client.Models;
using EDO_FOMS.Domain.Enums;
using System;
using EDO_FOMS.Application.Responses.Agreements;
using EDO_FOMS.Client.Infrastructure.Managers;
using EDO_FOMS.Client.Infrastructure.Model;

namespace EDO_FOMS.Client.Pages
{
    public partial class InProgressDialog
    {
        [Inject] private IDocumentManager DocManager { get; set; }
        [Inject] private IIconManager IconManager { get; set; }

        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        //[Parameter] public AddEditDocumentCommand _doc { get; set; } = new();
        [Parameter] public DocModel Doc { get; set; }

        private MudTable<AgreementsProgressResponse> _mudTable;
        private List<AgreementsProgressResponse> _agreements = new();
        private AgreementsProgressResponse _agreement;

        private bool _loaded = false;
        private bool _isAnswered = false;
        private bool _isCanceled = false;
        private bool _isMain = false;

        private string _action = "";

        private int tz;
        private int delay;
        private int duration;

        protected override async Task OnInitializedAsync()
        {
            await _jsRuntime.InvokeVoidAsync("azino.Console", Doc, "DOC: ");

            tz = _stateService.Timezone;
            delay = _stateService.TooltipDelay;
            duration = _stateService.TooltipDuration;

            await GetDocAgreementsProgressAsync();

            if (Doc.AgreementId != null)
            {
                var a = _agreements.Find(a => a.AgreementId == Doc.AgreementId);

                if (a != null)
                {
                    _isAnswered = !(a.State == AgreementStates.Incoming || a.State == AgreementStates.Received
                        || a.State == AgreementStates.Opened) || Doc.Stage == DocStages.Archive;

                    _isCanceled = a.IsCanceled;

                    _isMain = (a.Action == ActTypes.Agreement || a.Action == ActTypes.Signing);

                    _action = a.Action.ToString();
                }
            }

            _loaded = true;
        }

        private async Task GetDocAgreementsProgressAsync()
        {
            var response = await DocManager.GetAgreementsProgressAsync(Doc.DocId, Doc.AgreementId);

            if (!response.Succeeded)
            {
                response.Messages.ForEach((m) => _snackBar.Add(m, Severity.Error));
                return;
            }

            var data = response.Data.OrderBy(a => a.Step).ThenBy(a => a.UserOrgId).ThenBy(a => a.AgreementId).ToList();

            _agreements.Clear();

            data.ForEach(a => {
                a.CreatedOn = a.CreatedOn?.AddHours(tz);
                _agreements.Add(a);
            });

            await _jsRuntime.InvokeVoidAsync("azino.Console", _agreements, "AGREEMENTS: ");
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
        private Func<AgreementsProgressResponse, int, string> RowStyle => (a, _) =>
        {
            return (a.UserOrgId == Doc.KeyOrgId) ? " font-style: italic;" : "";
        };

        private IconModel OrgTypeIcon(OrgTypes type) => IconManager.OrgTypeIcon(type);
    }
}
