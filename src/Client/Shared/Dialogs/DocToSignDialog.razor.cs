using EDO_FOMS.Client.Infrastructure.Managers.Doc.Document;
using EDO_FOMS.Client.Models;
using EDO_FOMS.Shared.Constants.Storage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Shared.Dialogs;

public partial class DocToSignDialog
{
    [Inject] private IDocumentManager DocManager { get; set; }

    private IBrowserFile _file;
    //private DocToSign _doc = new();

    private string thumbprint;
    private byte[] buffer;
    private string title;

    private bool onSigning = false;
    
    private int delay;
    private int duration;
    
    protected override async Task OnInitializedAsync()
    {
        thumbprint = await _localStorage.GetItemAsync<string>(StorageConstants.Local.UserThumbprint);

        delay = _stateService.TooltipDelay;
        duration = _stateService.TooltipDuration;
    }

    private void UploadFile(InputFileChangeEventArgs e)
    {
        _file = e.File;

        if (_file == null)
        {
            _snackBar.Add(_localizer["File not found"], Severity.Error);
            return;
        }

        title = Path.GetFileNameWithoutExtension(_file.Name);

        //_doc.Extension = Path.GetExtension(_file.Name);
        //_doc.FileName = Path.GetFileName(_file.Name);
        //_doc.Title = Path.GetFileNameWithoutExtension(_file.Name);
    }

    private async Task ToSign()
    {
        onSigning = true;

        buffer = new byte[_file.Size];
        await _file.OpenReadStream(_file.Size).ReadAsync(buffer);

        //var title = Path.GetFileNameWithoutExtension(_file.Name);

        var base64 = Convert.ToBase64String(buffer);
        //var url = $"data:{format};base64,{base64}";
        var sign = await _jsRuntime.InvokeAsync<string>("azino.SignCadesBES", thumbprint, base64, title);

        if (string.IsNullOrWhiteSpace(sign))
        {
            _snackBar.Add(_localizer["Signing failed"], Severity.Error);
            onSigning = false;
            return;
        }

        var data = Convert.FromBase64String(sign);
        //var fileStream = new MemoryStream(data);
        var fileName = $"{title}.sig";

        //using var streamRef = new DotNetStreamReference(stream: fileStream);

        await _jsRuntime.InvokeVoidAsync("azino.SaveFile", fileName, "application/octet-stream", data);

        _file = null;


        _snackBar.Add(_localizer["The document is signed"], Severity.Success);
        onSigning = false;
    }
}
