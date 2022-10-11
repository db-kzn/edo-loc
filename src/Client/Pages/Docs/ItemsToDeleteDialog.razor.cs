using EDO_FOMS.Application.Features.Documents.Commands;
using EDO_FOMS.Client.Infrastructure.Managers.Doc.Document;
using EDO_FOMS.Client.Models;
using EDO_FOMS.Domain.Enums;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Pages.Docs
{
    public partial class ItemsToDeleteDialog
    {
        [Inject] private IDocumentManager DocManager { get; set; }

        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        [Parameter] public DocModel[] Docs { get; set; } = System.Array.Empty<DocModel>();

        private int current = 0;
        private int total = 0;

        private bool processing = false;
        private bool cancel = false;

        protected override void OnInitialized()
        {
            total = Docs.Length;
            current = 0;
        }

        private async Task ItemsToDeleteAsync()
        {
            foreach (DocModel doc in Docs)
            {
                if (cancel) { Close(); }

                current++;
                StateHasChanged();

                ChangeDocStageCommand request = new() { Id = doc.DocId, Stage = DocStages.Deleted };

                var response = await DocManager.ChangeStageAsync(request);

                if (!response.Succeeded)
                {
                    response.Messages.ForEach(m => _snackBar.Add(m, Severity.Error));
                    continue;
                }
            }

            Close();
        }

        private async Task OnStart()
        {
            processing = true;
            await ItemsToDeleteAsync();
        }
        private void OnStop() => cancel = true;
        private void Close() => MudDialog.Close(DialogResult.Ok(true));
    }
}
