using EDO_FOMS.Client.JsResponse;
using LinqKit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
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

        //private List<Cert> CertList = new();
        private readonly List<JsCert> CorrectCertList = new();
        private int AllCertCount = 0;
        private int CorrectCertCount = 0;

        private object SelectedValue;

        protected override async Task OnInitializedAsync()
        {
            var сryptoState = await _jsRuntime.InvokeAsync<JsResult<JsCryptoState>>("azino.GetCryptoAbout");

            if (!сryptoState.Succeed)
            {
                OnInitializing = false;
                _snackBar.Add(сryptoState.Message, Severity.Error);
                return;
            }

            var data = сryptoState.Data;

            CadesPluginApiVersion = data.CadesPluginApiVersion;
            BrowserPluginVersion = data.BrowserPluginVersion;
            CspVersion = data.CspVersion;
            CspName = data.CspName;

            CadesPluginApiLoaded = CadesPluginApiVersion != "";
            BrowserPluginLoaded = BrowserPluginVersion != "";
            CspEnabled = CspVersion != "";

            var certList = await _jsRuntime.InvokeAsync<JsResult<List<JsCert>>>("azino.GetCertList", true);
            if (!certList.Succeed)
            {
                _snackBar.Add(certList.Message, Severity.Error);
                return;
            }

            certList.Data.Where(c => c.IsCorrect).ForEach(cert => CorrectCertList.Add(cert));

            CorrectCertCount = CorrectCertList.Count;

            await _jsRuntime.InvokeVoidAsync("azino.Console", CorrectCertList, "Correct Certs :");

            CheckIsSuccess = CadesPluginApiLoaded && BrowserPluginLoaded && CspEnabled && (CorrectCertCount > 0);
            OnChecking = ShowSuccessCheck || !CheckIsSuccess;

            OnInitializing = false;
        }

        async Task HelpOpen() => await _jsRuntime.InvokeVoidAsync("azino.LinkOpen", "https://www.cryptopro.ru/products/cades/plugin");

        void OnClickCert(MouseEventArgs e)
        {
            JsCert c = SelectedValue as JsCert;
            MudDialog.Close(DialogResult.Ok<JsCert>(c));
        }

        //void Close() => MudDialog.Close(DialogResult.Cancel());
    }
}
