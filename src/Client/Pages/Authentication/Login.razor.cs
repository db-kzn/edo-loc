using Blazored.FluentValidation;
using EDO_FOMS.Application.Models;
using EDO_FOMS.Application.Requests.Identity;
using EDO_FOMS.Application.Responses.Identity;
using EDO_FOMS.Client.Shared.Dialogs;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Constants.Storage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Pages.Authentication
{
    public partial class Login
    {
        //private FluentValidationValidator _fluentValidationValidator;
        //private bool Validated => _fluentValidationValidator.Validate(options => options.IncludeAllRuleSets());
        //private FakeCert _fakeCert;
        
        private readonly CertCheckRequest _certCheckModel = new();
        private readonly RegisterByCertRequest _certRegModel = new();
        private Cert cert = null;
        private List<OrgCardModel> orgCards = new();
        private object orgCard;
        private readonly TokenRequest _tokenModel = new();

        private bool onSubmiting = false;

        private enum LoginStates
        {
            Undefined = 0,

            ChooseCert = 1,
            ChooseOrgInn = 2,
            InputOrgInn = 3,

            AreYouChief = 4,
            ChiefRequired = 5,
            OrgTitle = 6,

            SignInReady = 7
        }
        private LoginStates CurrentState = LoginStates.ChooseCert;

        protected override async Task OnInitializedAsync()
        {
            var state = await _authStateProvider.GetAuthenticationStateAsync();

            if (state != new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())))
            {
                _navigationManager.NavigateTo("/");
            }
        }

        private async Task SubmitAsync()
        {
            onSubmiting = true;

            await _jsRuntime.InvokeAsync<string>("azino.Console", _certCheckModel.Thumbprint, "SignIn Thumbprint: ");
            var tokenСonnected = await _jsRuntime.InvokeAsync<bool>("azino.TokenCheck", _certCheckModel.Thumbprint);
            await _jsRuntime.InvokeVoidAsync("azino.Console", tokenСonnected, "Token Connected: ");

            if (!tokenСonnected)
            {
                _snackBar.Add(string.Format(_localizer["The token is not connected"]), Severity.Warning);
                onSubmiting = false;
                return;
            }

            var result = await _authManager.SignIn(_certCheckModel);

            if (result.Succeeded)
            {
                await _localStorage.SetItemAsync(StorageConstants.Local.UserThumbprint, _certCheckModel.Thumbprint);
                _snackBar.Add(string.Format(_localizer["Welcome {0}"], _tokenModel.Email), Severity.Success);
                _navigationManager.NavigateTo("/", true);
            }
            else
            {
                // org not found -> create org + owner
                // worker not found -> create user
                result.Messages.ForEach(m => _snackBar.Add(m, Severity.Error));
            }

            onSubmiting = false;
        }

        private async Task CertSelectAsync()
        {
            var parameters = new DialogParameters {{ "ShowSuccessCheck", false }};
            var dialog = _dialogService.Show<CertificatesDialog>(_localizer["System Check"], parameters);
            var res = await dialog.Result;

            if (res.Cancelled) { return; }
            await _jsRuntime.InvokeVoidAsync("azino.Console", res.Data);

            cert = res.Data as Cert;
            if (cert != null)
            {
                WriteCertToRegModel(cert);
                WriteCertToCheckModel(cert);
            }

            await CertCheckAsync();
        }

        private async Task CertCheckAsync()
        {
            var result = await _authManager.CertCheck(_certCheckModel);

            if (!result.Succeeded)
            {
                if (result.Messages.Count == 0)
                {
                    _snackBar.Add(_localizer["Unknown error"], Severity.Error);
                    return;
                }

                result.Messages.ForEach(m => _snackBar.Add(m, Severity.Error));
                return;
            }

            await _jsRuntime.InvokeVoidAsync("azino.Console", result.Data);

            CertCheckResponse response = result.Data;

            if (response == null)
            {
                _snackBar.Add(_localizer["Server did not respond"], Severity.Error);
            }
            else if (response.Result == CertCheckResults.IsValid)
            {
                _snackBar.Add(_localizer["The certificate has been verified. Login started"], Severity.Success);
                await SignInAsync(response); // SignIn. Check Token -> Send Cert
            }
            else if (response.Result == CertCheckResults.AddedNewCert)
            {
                _snackBar.Add(_localizer["The new certificate has been added to the user"], Severity.Success);
                await SignInAsync(response);  // SignIn. Check Token -> Send Cert
            }
            else if (response.Result == CertCheckResults.RegNewEmpl)
            {
                _snackBar.Add(_localizer["New employee registration required"], Severity.Warning);
                await SignInAsync(response); // Reg New Employee -> Send Cert
            }
            else if (response.Result == CertCheckResults.RegOrgByInn)
            {
                _snackBar.Add(_localizer["Organization registration required"], Severity.Warning);
                await AreYouChiefAsync(response); // Are you chief?
            }
            else if (response.Result == CertCheckResults.RegNewUser)
            {
                _snackBar.Add(_localizer["New user registration required"], Severity.Warning);
                // Reg New User. If INN is NEW -> Are you chief? Else -> Add New Employee
                InputOrgInn();
            }
            else if (response.Result == CertCheckResults.RegOneBySnils)
            {
                _snackBar.Add(_localizer["User organization found"], Severity.Warning);
                // Chose INN. Selected -> Reg new Employee || new Cert. Else -> Input INN
                ChooseOrgInn(response);
            }
            else if (response.Result == CertCheckResults.RegSomeBySnils)
            {
                _snackBar.Add(_localizer["Multiple user organizations found"], Severity.Warning);
                // Chose INN. Selected -> Reg new Employee || new Cert. Else -> Input INN
                ChooseOrgInn(response);
            }
            else if (response.Result == CertCheckResults.IsBlocked)
            {
                _snackBar.Add(_localizer["Certificate not active. Administrator verification required"], Severity.Error);
            }
            else if (response.Result == CertCheckResults.UserNotFound)
            {
                _snackBar.Add(_localizer["The user specified in the certificate was not found"], Severity.Error);
            }
            else if (response.Result == CertCheckResults.UserIsBlocked)
            {
                _snackBar.Add(_localizer["The user is not active. Administrator verification required"], Severity.Error);
            }
            else if (response.Result == CertCheckResults.OrgNotFound)
            {
                _snackBar.Add(_localizer["Certificate user organization not found"], Severity.Error);
            }
            else if (response.Result == CertCheckResults.OrgIsBlocked)
            {
                _snackBar.Add(_localizer["The organization is not active. Admin Verification Required"], Severity.Error);
            }
        }

        private async Task SignInAsync(CertCheckResponse response)
        {
            // IsValid      + SignIn
            // AddedNewCert + Added New Cert by INN LE
            // RegNewEmpl   - Add New Empl by INN LE

            if (response.Result == CertCheckResults.RegNewEmpl)
            {
                await SignUpAsync();
            }
            else
            {
                CurrentState = LoginStates.SignInReady;
            }
        }

        private void ChooseOrgInn(CertCheckResponse response)
        {
            // (RegOneBySnils, RegSomeBySnils) => LoginStates.ChooseOrgInn => RegCert || InputOrgInn

            orgCards.Clear();
            orgCards.AddRange(response.OrgCards);

            CurrentState = LoginStates.ChooseOrgInn;
        }

        private async Task OnClickCardAsync(MouseEventArgs e)
        {
            OrgCardModel card = orgCard as OrgCardModel;

            _certCheckModel.OrgInn = card.Inn;

            _certRegModel.InnLe = card.Inn;
            _certRegModel.Org = card.Name;
            if (string.IsNullOrWhiteSpace(_certRegModel.Title)) { _certRegModel.Title = "Сотрудник"; }

            await CertCheckAsync();
        }

        private void InputOrgInn()
        {
            // ((RegOneBySnils, RegSomeBySnils) => RegNewUser) => RegEmpl || LoginStates.AreYouChief ?
            if (string.IsNullOrWhiteSpace(_certRegModel.Org)) { _certRegModel.Org = "ПО ИНН"; }
            if (string.IsNullOrWhiteSpace(_certRegModel.Title)) { _certRegModel.Title = "Сотрудник"; }

            CurrentState = LoginStates.InputOrgInn;
        }

        private async Task CheckOrgInnAsync()
        {
            var response = await _userManager.GetUserOrgExists(_certRegModel.InnLe);

            if (!response.Succeeded)
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }

                return;
            }

            var exists = response.Data;

            if (exists)
            {
                _certCheckModel.OrgInn = _certRegModel.InnLe;
                await CertCheckAsync();
            }
            else
            {
                CurrentState = LoginStates.AreYouChief;
            }
        }

        private async Task AreYouChiefAsync(CertCheckResponse response)
        {
            // (RegOrgByInn) => LoginStates.AreYouChief ? => Need Chief || RegOrg & Chief

            await _jsRuntime.InvokeAsync<string>("azino.GetCertDetails", _certCheckModel.Thumbprint);
            CurrentState = LoginStates.AreYouChief;
        }

        private async Task SignUpAsync()
        {
            await _jsRuntime.InvokeAsync<string>("azino.Console", _certRegModel, "SignUp CERT: ");

            var response = await _userManager.RegisterByCertAsync(_certRegModel);

            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                //_navigationManager.NavigateTo("/login");
                CurrentState = LoginStates.SignInReady;
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private void NoIamNotChief() { CurrentState = LoginStates.ChiefRequired; }
        private void YesIamChiefAsync()
        { 
            _certRegModel.BaseRole = UserBaseRoles.Chief;
            CurrentState = LoginStates.OrgTitle;
            //await SignUpAsync();
        }
        private void Ok() { CurrentState = LoginStates.ChooseCert; }

        private void WriteCertToRegModel(Cert cert)
        {
            _certRegModel.InnLe = cert.Subject.InnLe;
            _certRegModel.Snils = cert.Subject.Snils;
            _certRegModel.Inn = cert.Subject.Inn;

            _certRegModel.Title = cert.Subject.Title;
            _certRegModel.Surname = cert.Subject.Surname;
            _certRegModel.GivenName = cert.Subject.GivenName;

            _certRegModel.OrgType = OrgTypes.MO;
            _certRegModel.BaseRole = UserBaseRoles.Employee;

            _certRegModel.Ogrn = cert.Subject.Ogrn;
            _certRegModel.Name = cert.Subject.Name;
            _certRegModel.Org = cert.Subject.Org.Replace("\"", "");
            _certRegModel.Email = cert.Subject.Email;

            _certRegModel.Thumbprint = cert.Thumbprint;
            _certRegModel.FromDate = Convert.ToDateTime(cert.FromDate);
            _certRegModel.TillDate = Convert.ToDateTime(cert.TillDate);
        }
        private void WriteCertToCheckModel(Cert cert)
        {
            _certCheckModel.OrgName = cert.Subject.Org;
            _certCheckModel.OrgInn = cert.Subject.InnLe;

            _certCheckModel.Surname = cert.Subject.Surname;
            _certCheckModel.GivenName = cert.Subject.GivenName;

            _certCheckModel.Snils = cert.Subject.Snils;
            _certCheckModel.Thumbprint = cert.Thumbprint;

            _certCheckModel.FromDate = Convert.ToDateTime(cert.FromDate);
            _certCheckModel.TillDate = Convert.ToDateTime(cert.TillDate);
        }

        //private bool _passwordVisibility;
        //private InputType _passwordInput = InputType.Password;
        //private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

        //void TogglePasswordVisibility()
        //{
        //    if (_passwordVisibility)
        //    {
        //        _passwordVisibility = false;
        //        _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
        //        _passwordInput = InputType.Password;
        //    }
        //    else
        //    {
        //        _passwordVisibility = true;
        //        _passwordInputIcon = Icons.Material.Filled.Visibility;
        //        _passwordInput = InputType.Text;
        //    }
        //}

        //private readonly List<FakeCert> FakeCerts = new()
        //{
        //    new() { OrgInn = "1653006786", Snils = "12345678900", Thumbprint = "01234567890123456789012345678901234-0000", Name = "Фонд - Руководитель" },
        //    new() { OrgInn = "1653006786", Snils = "12345678901", Thumbprint = "01234567890123456789012345678901234-0001", Name = "Фонд - Админ ЭДО" },
        //    new() { OrgInn = "1653006786", Snils = "12345678904", Thumbprint = "01234567890123456789012345678901234-0004", Name = "Фонд - Управляющий" },
        //    new() { OrgInn = "1653006786", Snils = "12345678902", Thumbprint = "01234567890123456789012345678901234-0002", Name = "Фонд - Сотрудник" },
        //    new() { OrgInn = "1653006786", Snils = "12345678903", Thumbprint = "01234567890123456789012345678901234-0003", Name = "Фонд - Пользователь" },

        //    new() { OrgInn = "1600000100", Snils = "12345678910", Thumbprint = "01234567890123456789012345678901234-0100", Name = "CMO 1 - Руководитель" },
        //    new() { OrgInn = "1600000100", Snils = "12345678911", Thumbprint = "01234567890123456789012345678901234-0101", Name = "CMO 1 - Управляющий" },
        //    new() { OrgInn = "1600000100", Snils = "12345678912", Thumbprint = "01234567890123456789012345678901234-0102", Name = "CMO 1 - Сотрудник" },
        //    new() { OrgInn = "1600000100", Snils = "12345678913", Thumbprint = "01234567890123456789012345678901234-0103", Name = "CMO 1 - Пользователь" },

        //    new() { OrgInn = "1600000200", Snils = "12345678920", Thumbprint = "01234567890123456789012345678901234-0200", Name = "CMO 2 - Руководитель" },
        //    new() { OrgInn = "1600000200", Snils = "12345678921", Thumbprint = "01234567890123456789012345678901234-0201", Name = "CMO 2 - Управляющий" },
        //    new() { OrgInn = "1600000200", Snils = "12345678922", Thumbprint = "01234567890123456789012345678901234-0202", Name = "CMO 2 - Сотрудник" },
        //    new() { OrgInn = "1600000200", Snils = "12345678923", Thumbprint = "01234567890123456789012345678901234-0203", Name = "CMO 2 - Пользователь" },

        //    new() { OrgInn = "1600000300", Snils = "12345678930", Thumbprint = "01234567890123456789012345678901234-0300", Name = "CMO 3 - Руководитель" },
        //    new() { OrgInn = "1600000300", Snils = "12345678931", Thumbprint = "01234567890123456789012345678901234-0301", Name = "CMO 3 - Управляющий" },
        //    new() { OrgInn = "1600000300", Snils = "12345678932", Thumbprint = "01234567890123456789012345678901234-0302", Name = "CMO 3 - Сотрудник" },
        //    new() { OrgInn = "1600000300", Snils = "12345678933", Thumbprint = "01234567890123456789012345678901234-0303", Name = "CMO 3 - Пользователь" },

        //    new() { OrgInn = "1600100000", Snils = "12345678100", Thumbprint = "01234567890123456789012345678901234-1000", Name = "MO 1 - Руководитель" },
        //    new() { OrgInn = "1600100000", Snils = "12345678101", Thumbprint = "01234567890123456789012345678901234-1001", Name = "MO 1 - Управляющий" },
        //    new() { OrgInn = "1600100000", Snils = "12345678102", Thumbprint = "01234567890123456789012345678901234-1002", Name = "MO 1 - Сотрудник" },
        //    new() { OrgInn = "1600100000", Snils = "12345678103", Thumbprint = "01234567890123456789012345678901234-1003", Name = "MO 1 - Пользователь" },

        //    new() { OrgInn = "1600200000", Snils = "12345678200", Thumbprint = "01234567890123456789012345678901234-2000", Name = "MO 2 - Руководитель" },
        //    new() { OrgInn = "1600200000", Snils = "12345678201", Thumbprint = "01234567890123456789012345678901234-2001", Name = "MO 2 - Управляющий" },
        //    new() { OrgInn = "1600200000", Snils = "12345678202", Thumbprint = "01234567890123456789012345678901234-2002", Name = "MO 2 - Сотрудник" },
        //    new() { OrgInn = "1600200000", Snils = "12345678203", Thumbprint = "01234567890123456789012345678901234-2003", Name = "MO 2 - Пользователь" },

        //    new() { OrgInn = "1616000000", Snils = "12345670000", Thumbprint = "01234567890123456789012345678901234-X000", Name = "Без организации - Некто" }
        //};

        //readonly Func<FakeCert, string> converter = p => p?.Name;

        //void FakeChanged ()
        //{
        //    _certModel.OrgInn = _fakeCert.OrgInn;
        //    _certModel.Snils = _fakeCert.Snils;
        //    _certModel.Thumbprint = _fakeCert.Thumbprint;
        //}

        //private class FakeCert
        //{
        //    public string Name { get; set; }
        //    public string OrgInn { get; set; }
        //    public string Snils { get; set; }
        //    public string Thumbprint { get; set; }
        //}
    }
}
