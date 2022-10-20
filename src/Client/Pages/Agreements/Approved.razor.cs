using EDO_FOMS.Application.Requests.Agreements;
using EDO_FOMS.Application.Responses.Agreements;
using EDO_FOMS.Client.Infrastructure.Managers.Doc.Document;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Pages.Agreements
{
    public partial class Approved
    {
        [Inject] private IDocumentManager DocManager { get; set; }

        private MudTable<EmployeeAgreementsResponse> _mudTable;
        private List<EmployeeAgreementsResponse> _employeeAgreements;
        private EmployeeAgreementsResponse _employeeAgreement = new();

        private string _searchString = "";
        private bool _loaded = false;
        private int tz;

        private ClaimsPrincipal _authUser;
        private bool _canDocsEdit;

        private int delay;
        private int duration;

        protected override async Task OnInitializedAsync()
        {
            _authUser = await _authManager.CurrentUser();
            //_authUser = await _authStateProvider.GetAuthenticationStateProviderUserAsync();
            _canDocsEdit = (await _authService.AuthorizeAsync(_authUser, Permissions.Documents.Edit)).Succeeded;

            tz = _stateService.Timezone;

            //var state = await _authStateProvider.GetAuthenticationStateAsync();
            //var user = state.User;
            //if (user == null) { return; }
            //if (user.Identity?.IsAuthenticated == true) { _authUserId = user.GetUserId(); }

            delay = _stateService.TooltipDelay;
            duration = _stateService.TooltipDuration;

            await GetAgreementsAsync(AgreementStates.Approved);
            _loaded = true;
        }

        private async Task GetAgreementsAsync(AgreementStates state)
        {
            var request = new GetPagedAgreementsRequest(10, 1, "", null, AgreementStates.Undefined, false);
            var response = await DocManager.GetEmployeeAgreementsAsync(request);

            if (!response.Succeeded)
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }

                return;
            }

            _employeeAgreements = response.Data;
            
            _employeeAgreements.ForEach(a =>
            {
                a.CreatedOn = a.CreatedOn.AddHours(tz);
                a.DocCreatedOn = a.DocCreatedOn.AddHours(tz);
            });

            await _jsRuntime.InvokeVoidAsync("azino.Console", _employeeAgreements);
        }
    }
}
