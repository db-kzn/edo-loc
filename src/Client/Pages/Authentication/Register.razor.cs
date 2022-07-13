using Blazored.FluentValidation;
using EDO_FOMS.Application.Models;
using EDO_FOMS.Application.Requests.Identity;
using EDO_FOMS.Client.Shared.Dialogs;
using EDO_FOMS.Domain.Enums;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Pages.Authentication
{
    public partial class Register
    {
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

        private RegisterByCertRequest _certModel = new();

        //private CertRequest _certModel = new();
        //private string error = "";
        private Cert cert = new();
        private bool certSelected = false;

        private bool _passwordVisibility;
        private InputType _passwordInput = InputType.Password;
        private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

        private void TogglePasswordVisibility()
        {
            if (_passwordVisibility)
            {
                _passwordVisibility = false;
                _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
                _passwordInput = InputType.Password;
            }
            else
            {
                _passwordVisibility = true;
                _passwordInputIcon = Icons.Material.Filled.Visibility;
                _passwordInput = InputType.Text;
            }
        }

        private async Task SubmitAsync()
        {
            if (_certModel.InnLe.Length != 10)
            {
                _snackBar.Add("ИНН обязателен и должен содержать 10 цифр.", Severity.Error);
                return;
            }

            var userOrgExists = await _userManager.GetUserOrgExists(_certModel.InnLe);
            var exists = userOrgExists.Data;

            await _jsRuntime.InvokeVoidAsync("azino.Console", userOrgExists);

            if (!exists)
            {
                DialogOptions closeButton = new() { CloseButton = false };
                var dialog = _dialogService.Show<AreYouChiefDialog>(@_localizer["Are you org cheif?"], closeButton);
                var res = await dialog.Result;

                if (res.Cancelled || !(bool)res.Data) {
                    _dialogService.Show<OrgNotFoundDialog>(@_localizer["Organization not found"]);
                    return;
                }

                _certModel.BaseRole = UserBaseRoles.Chief;
            }

            var response = await _userManager.RegisterByCertAsync(_certModel);
            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                _navigationManager.NavigateTo("/login");
                //_registerUserModel = new RegisterRequest();
                _certModel = new RegisterByCertRequest();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private async Task CertSelect()
        {
            var parameters = new DialogParameters {{ "ShowSuccessCheck", false }};

            var dialog = _dialogService.Show<CertificatesDialog>(@_localizer["System Check"], parameters);
            var res = await dialog.Result;

            if (res.Cancelled) { return; }
            await _jsRuntime.InvokeVoidAsync("azino.Console", res.Data);

            cert = res.Data as Cert;
            certSelected = (cert != null); 

            if (certSelected)
            {
                WriteCertToModel(cert);
                // Check Cert on Server -> Return Status
            }
        }

        private void WriteCertToModel(Cert cert)
        {
            _certModel.InnLe = cert.Subject.InnLe;
            _certModel.Snils = cert.Subject.Snils;
            _certModel.Inn = cert.Subject.Inn;

            _certModel.Title = cert.Subject.Title;
            _certModel.Surname = cert.Subject.Surname;
            _certModel.GivenName = cert.Subject.GivenName;

            _certModel.OrgType = OrgTypes.MO;
            _certModel.BaseRole = UserBaseRoles.Employee;

            _certModel.Ogrn = cert.Subject.Ogrn;
            _certModel.Name = cert.Subject.Name;
            _certModel.Org = cert.Subject.Org;
            _certModel.Email = cert.Subject.Email;

            _certModel.Thumbprint = cert.Thumbprint;
            _certModel.FromDate = Convert.ToDateTime(cert.FromDate);
            _certModel.TillDate = Convert.ToDateTime(cert.TillDate);
        }
    }
}
