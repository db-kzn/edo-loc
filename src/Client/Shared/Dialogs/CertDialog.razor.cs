using EDO_FOMS.Application.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using System.Collections.Generic;
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

    private List<Cert> CertList = new();
    private List<Cert> CorrectCertList = new();
    private int AllCertCount = 0;
    private int CorrectCertCount = 0;

    private object SelectedValue;

    protected override async Task OnInitializedAsync()
    {
        CadesPluginApiVersion = await _jsRuntime.InvokeAsync<string>("azino.CadesPluginApiVersion");
        CadesPluginApiLoaded = CadesPluginApiVersion != "";

        BrowserPluginVersion = (CadesPluginApiLoaded) ? await _jsRuntime.InvokeAsync<string>("azino.BrowserPluginVersion") : "";
        BrowserPluginLoaded = BrowserPluginVersion != "";

        await _jsRuntime.InvokeVoidAsync("azino.Console", BrowserPluginVersion);            

        CspVersion = (BrowserPluginLoaded) ? await _jsRuntime.InvokeAsync<string>("azino.CspVersion") : "";
        CspEnabled = CspVersion != "";

        await _jsRuntime.InvokeVoidAsync("azino.Console", CspEnabled);

        CspName = (CspEnabled) ? await _jsRuntime.InvokeAsync<string>("azino.CspName") : "";

        await _jsRuntime.InvokeVoidAsync("azino.Console", CspName);

        CertList = (CspEnabled) ? await _jsRuntime.InvokeAsync<List<Cert>>("azino.GetCerts") : new();
        AllCertCount = CertList.Count;
        CorrectCertCount = 0;

        foreach (var cert in CertList)
        {
            if (cert.IsCorrect)
            {
                CorrectCertCount++;
                CorrectCertList.Add(cert);
                //CorrectCertList.Add(cert);
            }
        }

        await _jsRuntime.InvokeVoidAsync("azino.Console", CertList);

        OnInitializing = false;
    }

    [CascadingParameter] MudDialogInstance MudDialog { get; set; }

    void Close() => MudDialog.Close(DialogResult.Cancel());
    void HelpOpen() => _jsRuntime.InvokeVoidAsync("azino.LinkOpen", "https://www.cryptopro.ru/products/cades/plugin");

    void ShowCerts()
    {
        OnChecking = !OnChecking;
        //MudDialog.Close(DialogResult.Ok(true));
        MudDialog.SetTitle("Current time is: ");
    }

    void OnClickCert(MouseEventArgs e) {
        Cert c = SelectedValue as Cert;
        MudDialog.Close(DialogResult.Ok<Cert>(c));
        //_jsRuntime.InvokeVoidAsync("azino.Console", c);        
    }
}    
