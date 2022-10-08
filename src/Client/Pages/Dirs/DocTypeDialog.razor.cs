using EDO_FOMS.Application.Features.Directories.Commands;
using EDO_FOMS.Client.Infrastructure.Managers.Dir;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Pages.Dirs
{
    public partial class DocTypeDialog
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public AddEditDocTypeCommand AddEditDocTypeCommand { get; set; } = new();
        [Inject] private IDirectoryManager DirManager { get; set; }

        private async Task SaveAsync()
        {
            await _jsRuntime.InvokeVoidAsync("azino.Console", AddEditDocTypeCommand, "AddEditDocTypeCommand");

            var response = await DirManager.DocTypePostAsync(AddEditDocTypeCommand);

            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
            }
            else
            {
                response.Messages.ForEach(m => _snackBar.Add(m, Severity.Error));
            }

            MudDialog.Close();
        }
    }
}
