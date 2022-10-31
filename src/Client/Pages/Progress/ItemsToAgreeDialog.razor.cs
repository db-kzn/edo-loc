using EDO_FOMS.Application.Features.Agreements.Commands;
using EDO_FOMS.Client.Infrastructure.Managers.Doc.Document;
using EDO_FOMS.Client.JsResponse;
using EDO_FOMS.Client.Models;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Constants.Storage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Pages.Progress
{
    public partial class ItemsToAgreeDialog
    {
        [Inject] private IDocumentManager DocManager { get; set; }

        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        [Parameter] public AgreementModel[] Agrs { get; set; } = System.Array.Empty<AgreementModel>();

        private int current = 0;
        private int total = 0;

        private bool processing = false;
        private bool cancel = false;

        private string thumbprint;
        private string base64;
        private string sign;

        protected override async Task OnInitializedAsync()
        {
            total = Agrs.Length;
            current = 0;
            thumbprint = await _localStorage.GetItemAsync<string>(StorageConstants.Local.UserThumbprint);
        }

        private async Task ItemsToSignAsync()
        {
            foreach (AgreementModel agr in Agrs)
            {
                if (cancel) { Close(); }

                current++;
                StateHasChanged();

                base64 = await DocManager.GetBase64Async(agr.DocURL);
                var signed = await _jsRuntime.InvokeAsync<JsResult<string>>("azino.SignCadesBES", thumbprint, base64, agr.DocTitle);
                sign = signed.Succeed ? signed.Data : "";

                if (string.IsNullOrWhiteSpace(sign)) { continue; }

                AgreementSignedCommand cmdSigned = new()
                {
                    AgreementId = agr.AgreementId,
                    EmplId = agr.EmplId,
                    EmplOrgId = agr.EmplOrgId,
                    DocId = agr.DocId,
                    //Thumbprint = thumbprint,

                    Data = Convert.FromBase64String(sign) //Encoding.ASCII.GetBytes(base64)
                };
                var response = await DocManager.PostAgreementSignedAsync(cmdSigned);
                if (!response.Succeeded) { continue; }

                AgreementAnswerCommand cmdAnswer = new()
                {
                    Id = agr.AgreementId,
                    State = AgreementStates.Approved,
                    Remark = "",
                    Members = new(),
                    Thumbprint = thumbprint,

                    URL = "",
                    UploadRequest = new()
                };
                var result = await DocManager.PostAgreementAnswerAsync(cmdAnswer);
                if (!result.Succeeded) { continue; }
            }

            Close();
        }

        private async Task OnStart()
        {
            processing = true;
            await ItemsToSignAsync();
        }
        private void OnStop() => cancel = true;
        private void Close() => MudDialog.Close(DialogResult.Ok(true));
    }
}
