using EDO_FOMS.Application.Features.Orgs.Commands;
using EDO_FOMS.Application.Responses.Identity;
using EDO_FOMS.Client.Infrastructure.Managers.System;
using EDO_FOMS.Client.Infrastructure.Model.Admin;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Pages.Admin
{
    public partial class OrgCard
    {
        [Parameter] public int? OrgId { get; set; }
        [Inject] private IAdminManager AdmManager { get; set; }

        private int tz;
        private int delay;
        private int duration;

        private MudTabs _tabs;
        //private int tabIndex = 0;

        private OrgCardModel Org { get; set; } = new();
        private MudTable<OrgCardUserModel> _mudTable;
        private OrgCardUserModel _selectedUser = new();

        private ClaimsPrincipal _authUser;
        private bool _canSystemEdit = false;

        private OrgTypes _orgType;

        protected override async Task OnInitializedAsync()
        {
            _authUser = await _authStateProvider.GetAuthenticationStateProviderUserAsync();
            _canSystemEdit = (await _authService.AuthorizeAsync(_authUser, Permissions.System.Edit)).Succeeded;

            tz = _stateService.Timezone;
            delay = _stateService.TooltipDelay;
            duration = _stateService.TooltipDuration;

            await GetOrgCardAsync();
        }

        private async Task GetOrgCardAsync()
        {
            if (OrgId is null || OrgId == 0) { return; }

            // Событме - данные запрошены у сервера

            var response = await AdmManager.GetOrgCardAsync((int)OrgId);
            await _jsRuntime.InvokeVoidAsync("azino.Console", response, "Org Card Response");

            if (!response.Succeeded)
            {
                response.Messages.ForEach((m) => _snackBar.Add(m, Severity.Error));
                Close();
            }

            var orgCard = response.Data;

            Org.Id = orgCard.Id;

            Org.Inn = orgCard.Inn;
            Org.Code = orgCard.Code;
            Org.Name = orgCard.Name;
            Org.ShortName = orgCard.ShortName;

            Org.IsPublic = orgCard.IsPublic;
            Org.Type = orgCard.Type;
            Org.State = orgCard.State;

            Org.Phone = orgCard.Phone;
            Org.Email = orgCard.Email;
            Org.CreatedOn = orgCard.CreatedOn;

            _orgType = Org.Type;

            orgCard.Employees.ForEach(e => Org.Users.Add(NewOrgCardUserModel(e)));

            await _jsRuntime.InvokeVoidAsync("azino.Console", Org, "Org Card Model");


            StateHasChanged();
            // Событие - данные загружены
            //return;
        }
        private async Task SaveAsync()
        {
            if (DataErrorCount() > 0) { return; }

            var addEditOrgCommand = new AddEditOrgCommand() // OrgCardModel -> AddEditOrgCommand
            {
                Id = Org.Id,
                
                Inn = Org.Inn,
                OmsCode = Org.Code,
                Name = Org.Name,
                ShortName = Org.ShortName,
                
                IsPublic = Org.IsPublic,
                Type = Org.Type,
                State = Org.State,
                
                Phone = Org.Phone,
                Email = Org.Email
            };

            var response = await AdmManager.AddEditOrgAsync(addEditOrgCommand);

            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);

                if (_orgType != Org.Type)
                {
                    await AdmManager.UpdateUsersOrgTypeAsync(new()
                    {
                        OrgId = Org.Id,
                        InnLe = Org.Inn,
                        OrgType = Org.Type
                    });
                }

                Close();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
        private int DataErrorCount()
        {
            var error = 0;

            if (Org.Inn.Length != 10 && Org.Inn.Length != 12)
            {
                error++;
                _snackBar.Add(_localizer["INN mast be 10 or 12 symbols"], Severity.Warning);
            }
            if (Org.Ogrn.Length > 13)
            {
                error++;
                _snackBar.Add(_localizer["OGRN max length 13 symbols"], Severity.Warning);
            }
            if (Org.Code.Length > 6)
            {
                error++;
                _snackBar.Add(_localizer["NSI Code max length 6 symbols"], Severity.Warning);
            }
            if (string.IsNullOrWhiteSpace(Org.Name))
            {
                error++;
                _snackBar.Add(_localizer["Name is requred"], Severity.Warning);
            }
            if (Org.Name.Length > 500)
            {
                error++;
                _snackBar.Add(_localizer["Name max length 500 symbols"], Severity.Warning);
            }
            if (Org.ShortName.Length > 32)
            {
                error++;
                _snackBar.Add(_localizer["Short Name max length 32 symbols"], Severity.Warning);
            }

            return error;
        }

        private void Close() => NavigateToOrgs();
        private void NavigateToOrgs() => _navigationManager.NavigateTo($"/admin/orgs");
        private void OnUserRowClick()
        {
            if (_selectedUser is not null)
            {
                _navigationManager.NavigateTo($"/admin/users/user-card/{_selectedUser.Id}");
            }
        }

        private OrgCardUserModel NewOrgCardUserModel(UserResponse u)
        {
            return new()
            {
                Id = u.Id,
                InnLe = u.InnLe,
                Snils = u.Snils,
                Inn = u.Inn,

                Title = u.Title,
                UserName = u.UserName,
                Surname = u.Surname,
                GivenName = u.GivenName,

                OrgType = u.OrgType,
                BaseRole = u.BaseRole,
                IsActive = u.IsActive,

                Email = u.Email,
                EmailConfirmed = u.EmailConfirmed,
                PhoneNumber = u.PhoneNumber,
                PhoneNumberConfirmed = u.PhoneNumberConfirmed,

                ProfilePictureDataUrl = u.ProfilePictureDataUrl,
                CreatedOn = u.CreatedOn,
            };
        }
    }
}
