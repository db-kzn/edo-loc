using EDO_FOMS.Application.Models.Dir;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EDO_FOMS.Client.Pages.Dirs
{
    public partial class RouteStepDialog
    {
        [CascadingParameter]
        private MudDialogInstance MudDialog { get; set; }
        [Parameter]
        public RouteStageStepModel Step { get; set; }

        private void Ok() => MudDialog.Close(DialogResult.Ok(Step));
        private void Delete() => MudDialog.Close(DialogResult.Ok<RouteStageStepModel>(null));
    }
}
