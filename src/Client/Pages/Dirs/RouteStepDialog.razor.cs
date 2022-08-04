using EDO_FOMS.Domain.Entities.Dir;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Pages.Dirs
{
    public partial class RouteStepDialog
    {
        [CascadingParameter]
        private MudDialogInstance MudDialog { get; set; }
        [Parameter]
        public RouteStageStep Step { get; set; }

        private void Ok() => MudDialog.Close(DialogResult.Ok(Step));
        private void Delete() => MudDialog.Close(DialogResult.Ok<RouteStageStep>(null));
    }
}
