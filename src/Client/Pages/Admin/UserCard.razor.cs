using EDO_FOMS.Application.Features.Certs.Queries;
using EDO_FOMS.Application.Requests.Admin;
using EDO_FOMS.Client.Infrastructure.Managers.Orgs;
using EDO_FOMS.Client.Infrastructure.Managers.System;
using EDO_FOMS.Client.Infrastructure.Model.Admin;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Pages.Admin
{
    public partial class UserCard
    {
        [Inject] private IAdminManager AdmManager { get; set; }
        [Inject] private IOrgManager OrgManager { get; set; }

        [Parameter] public string UserId { get; set; }

        private int tz;
        private int delay;
        private int duration;

        private MudTabs _tabs;
        //private int tabIndex = 0;

        private UserCardModel User { get; set; } = new();
        private MudTable<UserCertModel> _mudTable;
        private readonly List<UserCertModel> userCerts = new();

        private ClaimsPrincipal _authUser;
        private bool _canSystemEdit = false;

        protected override async Task OnInitializedAsync()
        {
            _authUser = await _authStateProvider.GetAuthenticationStateProviderUserAsync();
            _canSystemEdit = (await _authService.AuthorizeAsync(_authUser, Permissions.System.Edit)).Succeeded;

            await _jsRuntime.InvokeVoidAsync("azino.Console", UserId, "User: ");

            tz = _stateService.Timezone;
            delay = _stateService.TooltipDelay;
            duration = _stateService.TooltipDuration;

            await GetUserCardAsync();
        }

        private async Task GetUserCardAsync()
        {

            if (string.IsNullOrWhiteSpace(UserId)) { return; }

            var response = await AdmManager.GetUserCardAsync(UserId);

            if (!response.Succeeded)
            {
                response.Messages.ForEach((m) => _snackBar.Add(m, Severity.Error));
                Close();
            }

            var uc = response.Data;
            await _jsRuntime.InvokeVoidAsync("azino.Console", uc, "User Card:");

            User.Id = uc.Id;
            User.InnLe = uc.InnLe;
            User.Snils = uc.Snils;

            User.Inn = uc.Inn;
            User.Title = uc.Title;

            User.Surname = uc.Surname;
            User.GivenName = uc.GivenName;

            User.BaseRole = uc.BaseRole;
            User.OrgType = uc.OrgType;

            User.OrgId = uc.OrgId;
            User.OrgName = uc.OrgName;
            User.OrgShort = uc.OrgShort;

            User.Email = uc.Email;
            User.PhoneNumber = uc.PhoneNumber;

            User.IsActive = uc.IsActive;
            User.EmailConfirmed = uc.EmailConfirmed;
            User.PhoneNumberConfirmed = uc.PhoneNumberConfirmed;

            User.ProfilePictureDataUrl = uc.ProfilePictureDataUrl;
            User.CreatedOn = uc.CreatedOn;

            await GetUserCertsAsync();
        }
        private async Task GetUserCertsAsync()
        {
            var response = await AdmManager.GetUserCertsAsync(UserId);

            if (!response.Succeeded)
            {
                response.Messages.ForEach((m) => _snackBar.Add(m, Severity.Error));
                return;
            }

            userCerts.Clear();
            response.Data.ForEach(c => userCerts.Add(NewUserCertModel(c)));

            await _jsRuntime.InvokeVoidAsync("azino.Console", userCerts, "Certs: ");
        }

        private async Task SaveAsync()
        {
            if (DataErrorCount() > 0) { return; }

            var response = await AdmManager.EditUserAsync(NewEditUserRequest(User));

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

            Close();
        }
        private int DataErrorCount()
        {
            var error = 0;

            if (User.InnLe.Length != 10 || User.InnLe.Length != 12)
            {
                error++;
                _snackBar.Add(_localizer["Org INN mast be 10 or 12 symbols"], Severity.Warning);
            }
            if (User.Snils.Length > 11)
            {
                error++;
                _snackBar.Add(_localizer["SNILS max length 11 symbols"], Severity.Warning);
            }
            if (User.EmailConfirmed && !new EmailAddressAttribute().IsValid(User.Email))
            {
                error++;
                _snackBar.Add(_localizer["e-mail address is wrong"], Severity.Error);
            }
            if (User.Inn.Length != 12)
            {
                error++;
                _snackBar.Add(_localizer["INN mast be 12 symbols"], Severity.Warning);
            }
            if (User.Title.Length > 200)
            {
                error++;
                _snackBar.Add(_localizer["Title max length 200 symbols"], Severity.Warning);
            }
            if (User.Surname.Length > 200)
            {
                error++;
                _snackBar.Add(_localizer["Surname max length 200 symbols"], Severity.Warning);
            }
            if (User.GivenName.Length > 200)
            {
                error++;
                _snackBar.Add(_localizer["GivenName max length 200 symbols"], Severity.Warning);
            }

            return error;
        }


        private void Close() => NavigateToUsers();
        private void NavigateToUsers() => _navigationManager.NavigateTo($"/admin/users");
        private void NavToOrg(int orgId) => _navigationManager.NavigateTo($"/admin/orgs/org-card/{orgId}");

        private UserCertModel NewUserCertModel(GetUserCertsResponse c)
        {
            return new()
            {
                Id = c.Id,

                Thumbprint = c.Thumbprint,
                UserId = c.UserId,
                Snils = c.Snils,

                IsActive = c.IsActive,
                SignAllowed = c.SignAllowed,
                IssuerInn = c.IssuerInn,

                SerialNumber = c.SerialNumber,
                Algorithm = c.Algorithm,
                Version = c.Version,

                FromDate = c.FromDate,
                TillDate = c.TillDate,
                CreatedOn = c.CreatedOn
            };
        }
        private EditUserRequest NewEditUserRequest(UserCardModel User)
        {
            return new()
            {
                Id = User.Id,
                InnLe = User.InnLe,
                Snils = User.Snils,
                Inn = User.Inn,

                Title = User.Title,
                Surname = User.Surname,
                GivenName = User.GivenName,

                OrgType = User.OrgType,
                BaseRole = User.BaseRole,

                IsActive = User.IsActive,
                Email = User.Email,
                EmailConfirmed = User.EmailConfirmed,
                PhoneNumber = User.PhoneNumber,
                PhoneNumberConfirmed = User.PhoneNumberConfirmed
            };
        }

        private string OrgTypeIcon(OrgTypes orgType) => OrgManager.OrgTypeIcon(orgType);
    }
}
