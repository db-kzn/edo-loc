using EDO_FOMS.Application.Features.Agreements.Commands;
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
    public partial class Inbox
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

            delay = _stateService.TooltipDelay;
            duration = _stateService.TooltipDuration;

            //var state = await _authStateProvider.GetAuthenticationStateAsync();
            //var user = state.User;
            //if (user == null) { return; }
            //if (user.Identity?.IsAuthenticated == true) { _authUserId = user.GetUserId(); }

            await GetAgreementsAsync(AgreementStates.Incoming);
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

        private async Task AddMembersAsync(EmployeeAgreementsResponse agreement)
        {
            await _jsRuntime.InvokeVoidAsync("azino.Console", agreement);
            
            //new EmployeeAgreementResponse { }
            var parameters = new DialogParameters() {{ nameof(MembersDialog._agreement), agreement }};

            var dialog = _dialogService.Show<MembersDialog>("", parameters); //, options

            var result = await dialog.Result;

            if (!result.Cancelled)
            {
                // ReloadAgreements ?
            }
        }

        private async Task OnRowClickAsync(TableRowClickEventArgs<EmployeeAgreementsResponse> e) =>
            await RemarkAnAgreementAsync(_employeeAgreement);

        private async Task RemarkAnAgreementAsync(EmployeeAgreementsResponse agreement)
        {
            await _jsRuntime.InvokeVoidAsync("azino.Console", agreement);

            // Show AgreementRemarkDialog
        }

        private async Task RejectAnAgreementAsync(EmployeeAgreementsResponse agreement)
        {
            var dialog = _dialogService.Show<AgreementRejectDialog>(""); //, options, parameters
            var result = await dialog.Result;

            if (result.Cancelled) { return; }

            AgreementAnswerCommand command = new()
            {
                Id = agreement.AgreementId,
                State = AgreementStates.Rejected,
                Remark = result.Data.ToString(),
                Members = new(),

                URL = "",
                UploadRequest = null
            };

            await _jsRuntime.InvokeVoidAsync("azino.Console", command);

            await SendAgreementAnswerAsync(command);
        }

        private async Task ApproveAnAgreementAsync(EmployeeAgreementsResponse agreement)
        {
            AgreementAnswerCommand command = new()
            {
                Id = agreement.AgreementId,
                State = AgreementStates.Approved,
                Remark = "",
                Members = new(),

                URL = "",
                UploadRequest = null
            };

            await SendAgreementAnswerAsync(command);
        }

        private async Task SignAnAgreementAsync(EmployeeAgreementsResponse agreement)
        {
            AgreementAnswerCommand command = new()
            {
                Id = agreement.AgreementId,
                State = AgreementStates.Signed,
                Remark = "",
                Members = new(),

                URL = "",
                UploadRequest = new()
            };

            await SendAgreementAnswerAsync(command);
        }

        private async Task SendAgreementAnswerAsync(AgreementAnswerCommand command)
        {
            var response = await DocManager.PostAgreementAnswerAsync(command);

            if (!response.Succeeded)
            {
                response.Messages.ForEach((m) => _snackBar.Add(m, Severity.Error));
                return;
            }

            _loaded = false;
            await GetAgreementsAsync(AgreementStates.Incoming);
            _loaded = true;
        }
    }
}
