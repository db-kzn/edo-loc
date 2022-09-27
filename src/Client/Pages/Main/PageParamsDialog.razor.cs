using EDO_FOMS.Application.Configurations;
using EDO_FOMS.Client.Infrastructure.Managers.System;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Pages.Main
{
    public partial class PageParamsDialog
    {
        [Inject] private IAdminManager AdmManager { get; set; }

        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        [Parameter] public HomeConfiguration _home { get; set; } = new();

        //protected override async Task OnInitializedAsync() { }

        private async Task SaveAsync()
        {
            await AdmManager.SaveHomeParamsAsync(_home);
            MudDialog.Close();
        }
        public void Cancel() => MudDialog.Cancel();
    }
}
