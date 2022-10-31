using EDO_FOMS.Application.Models;
using EDO_FOMS.Client.JsResponse;
using LinqKit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Shared.Dialogs;

public partial class CertDialog
{
    private bool OnInitializing = true;
    private bool OnChecking = true;

    private string CadesPluginApiVersion = "";
    private bool CadesPluginApiLoaded = false;
    private string BrowserPluginVersion = "";
    private bool BrowserPluginLoaded = false;
    private string CspVersion = "";
    private bool CspEnabled = false;
    private string CspName = "";

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

        var certList = await _jsRuntime.InvokeAsync<JsResult<List<JsCert>>>("azino.GetCertList");
        if (!certList.Succeed)
        {
            _snackBar.Add(certList.Message, Severity.Error);
            return;
        }

        certList.Data.Where(c => c.IsCorrect).ForEach(cert => CorrectCertList.Add(cert));

        CorrectCertCount = CorrectCertList.Count;

        await _jsRuntime.InvokeVoidAsync("azino.Console", CorrectCertList);

        OnInitializing = false;
    }

    [CascadingParameter] MudDialogInstance MudDialog { get; set; }

    void Close() => MudDialog.Close(DialogResult.Cancel());
    async Task HelpOpen() => await _jsRuntime.InvokeVoidAsync("azino.LinkOpen", "https://www.cryptopro.ru/products/cades/plugin");

    void ShowCerts()
    {
        OnChecking = !OnChecking;
        //MudDialog.Close(DialogResult.Ok(true));
        MudDialog.SetTitle("Current time is: ");
    }

    void OnClickCert(MouseEventArgs e)
    {
        Cert c = SelectedValue as Cert;
        MudDialog.Close(DialogResult.Ok<Cert>(c));
        //_jsRuntime.InvokeVoidAsync("azino.Console", c);
    }
}
