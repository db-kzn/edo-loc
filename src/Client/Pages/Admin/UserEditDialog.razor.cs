using Blazored.FluentValidation;
using EDO_FOMS.Application.Features.Certs.Queries;
using EDO_FOMS.Application.Models;
using EDO_FOMS.Application.Requests.Admin;
using EDO_FOMS.Client.Infrastructure.Managers.System;
using EDO_FOMS.Client.Shared.Dialogs;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Pages.Admin
{
    public partial class UserEditDialog
    {
        [Inject] private IAdminManager AdmManager { get; set; }

        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public EditUserRequest EditUser { get; set; } = new();

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => options.IncludeAllRuleSets());

        //private ClaimsPrincipal _authUser;
        //private bool _canSystemEdit = false;

        //private readonly IList<string> _userRoles = RoleConstants.List;
        //private readonly string _admin = RoleConstants.AdministratorRole;
        private Cert cert = new();
        private bool certSelected = false;

        private readonly List<GetUserCertsResponse> userCerts = new();
        private MudTable<GetUserCertsResponse> _mudTable;
        private int tabIndex = 0;

        private int delay;
        private int duration;

        protected override async Task OnInitializedAsync()
        {
            await _jsRuntime.InvokeVoidAsync("azino.Console", EditUser, "User: ");

            delay = _stateService.TooltipDelay;
            duration = _stateService.TooltipDuration;

            var response = await AdmManager.GetUserCertsAsync(EditUser.Id);

            if (!response.Succeeded)
            {
                response.Messages.ForEach((m) => _snackBar.Add(m, Severity.Error));
                return;
            }

            userCerts.Clear();
            response.Data.ForEach(c => userCerts.Add(c));

            await _jsRuntime.InvokeVoidAsync("azino.Console", userCerts, "Certs: ");
        }

        private async Task OnSubmit()
        {
            if (EditUser.EmailConfirmed && !new EmailAddressAttribute().IsValid(EditUser.Email))
            {
                _snackBar.Add(_localizer["email address is wrong"], Severity.Error);
                return;
            }

            var response = await AdmManager.EditUserAsync(EditUser);

            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }

            MudDialog.Close(DialogResult.Ok(true));
        }

        private async Task CertSelect()
        {
            var parameters = new DialogParameters { { "ShowSuccessCheck", false } };

            var dialog = _dialogService.Show<CertificatesDialog>(_localizer["System Check"], parameters);
            var res = await dialog.Result;

            if (res.Cancelled) { return; }
            await _jsRuntime.InvokeVoidAsync("azino.Console", res.Data);

            cert = res.Data as Cert;
            certSelected = (cert != null);

            if (certSelected)
            {
                EditUser.InnLe = cert.Subject.InnLe;
                EditUser.Snils = cert.Subject.Snils;
                EditUser.Inn = cert.Subject.Inn;

                EditUser.Title = cert.Subject.Title;
                EditUser.Surname = cert.Subject.Surname;
                EditUser.GivenName = cert.Subject.GivenName;

                //_editUser.OrgTypeIx = (int)OrgTypes.MO;
                //_editUser.BaseRoleIx = (int)UserBaseRoles.Employee;

                //_editUser.Ogrn = cert.Subject.Ogrn;
                //_editUser.OrgName = cert.Subject.Name;
                EditUser.Email = cert.Subject.Email;

                //_editUser.Thumbprint = cert.Thumbprint;
                //_editUser.FromDate = Convert.ToDateTime(cert.FromDate);
                //_editUser.TillDate = Convert.ToDateTime(cert.TillDate);
            }
        }

        void Close() => MudDialog.Close(DialogResult.Ok(true));
    }
}
