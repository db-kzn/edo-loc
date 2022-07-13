using Blazored.FluentValidation;
using EDO_FOMS.Application.Models;
using EDO_FOMS.Application.Requests.Admin;
using EDO_FOMS.Client.Infrastructure.Managers.System;
using EDO_FOMS.Domain.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Shared.Dialogs
{
    public partial class NewUserDialog
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => options.IncludeAllRuleSets());

        [Inject] private IAdminManager AdmManager { get; set; }
        //private ClaimsPrincipal _authUser;
        //private bool _canSystemEdit = false;

        private NewUserRequest _newUser = new();
        //private IList<string> _userRoles = RoleConstants.List;
        //private string _admin = RoleConstants.AdministratorRole;
        private Cert cert = new();
        private bool certSelected = false;

        private async Task OnSubmit()
        {
            var response = await AdmManager.AddUserAsync(_newUser);

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

            var dialog = _dialogService.Show<CertificatesDialog>(@_localizer["System Check"], parameters);
            var res = await dialog.Result;

            if (res.Cancelled) { return; }
            await _jsRuntime.InvokeVoidAsync("azino.Console", res.Data);

            cert = res.Data as Cert;
            certSelected = (cert != null);

            if (certSelected)
            {
                _newUser.InnLe = cert.Subject.InnLe;
                _newUser.Snils = cert.Subject.Snils;
                _newUser.Inn = cert.Subject.Inn;

                _newUser.Title = cert.Subject.Title;
                _newUser.Surname = cert.Subject.Surname;
                _newUser.GivenName = cert.Subject.GivenName;

                _newUser.OrgType = OrgTypes.MO;
                _newUser.BaseRole = UserBaseRoles.Employee;

                _newUser.Ogrn = cert.Subject.Ogrn;
                _newUser.OrgName = cert.Subject.Name;
                _newUser.Email = cert.Subject.Email;

                _newUser.Thumbprint = cert.Thumbprint;
                _newUser.FromDate = Convert.ToDateTime(cert.FromDate);
                _newUser.TillDate = Convert.ToDateTime(cert.TillDate);
            }
        }

        void Close() => MudDialog.Close(DialogResult.Ok(true));
    }
}
