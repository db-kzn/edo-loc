using EDO_FOMS.Application.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Shared.Dialogs
{
    public partial class CertificatesDialog
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public bool ShowSuccessCheck { get; set; }

        private bool OnInitializing = true;
        private bool OnChecking = true;
        private bool CheckIsSuccess = false;

        private bool CadesPluginApiLoaded = false;
        private bool BrowserPluginLoaded = false;
        private bool CspEnabled = false;

        private string CadesPluginApiVersion = "";
        private string BrowserPluginVersion = "";
        private string CspVersion = "";
        private string CspName = "";

        private List<Cert> CertList = new();
        private readonly List<Cert> CorrectCertList = new();
        private int AllCertCount = 0;
        private int CorrectCertCount = 0;

        private object SelectedValue;

        protected override async Task OnInitializedAsync()
        {
            CadesPluginApiVersion = await _jsRuntime.InvokeAsync<string>("azino.CadesPluginApiVersion");
            CadesPluginApiLoaded = CadesPluginApiVersion != "";

            if (CadesPluginApiLoaded)
            {
                BrowserPluginVersion = await _jsRuntime.InvokeAsync<string>("azino.BrowserPluginVersion");
                CspVersion = await _jsRuntime.InvokeAsync<string>("azino.CspVersion");
                CspName = await _jsRuntime.InvokeAsync<string>("azino.CspName");

                CertList = await _jsRuntime.InvokeAsync<List<Cert>>("azino.GetCerts");
                //await _jsRuntime.InvokeVoidAsync("azino.Console", CertList);

                BrowserPluginLoaded = BrowserPluginVersion != "";
                CspEnabled = CspVersion != "";
                AllCertCount = CertList.Count;
                CorrectCertCount = 0;

                foreach (var cert in CertList) {
                    if (cert.IsCorrect)
                    {
                        CorrectCertList.Add(cert); 
                    }
                }

                CorrectCertCount = CorrectCertList.Count;
                CheckIsSuccess = CadesPluginApiLoaded && BrowserPluginLoaded && CspEnabled && (CorrectCertCount > 0);
                OnChecking = ShowSuccessCheck || !CheckIsSuccess;
                OnInitializing = false;
            }
        }

        async Task HelpOpen() => await _jsRuntime.InvokeVoidAsync("azino.LinkOpen", "https://www.cryptopro.ru/products/cades/plugin");

        void OnClickCert(MouseEventArgs e) {
            Cert c = SelectedValue as Cert;
            MudDialog.Close(DialogResult.Ok<Cert>(c));
        }

        //void Close() => MudDialog.Close(DialogResult.Cancel());
    }
}
