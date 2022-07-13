using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using MudBlazor;
using Microsoft.JSInterop;
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using EDO_FOMS.Client.Infrastructure.Managers.Orgs;
using EDO_FOMS.Application.Features.Orgs.Commands;
using EDO_FOMS.Shared.Constants.Permission;
using EDO_FOMS.Client.Extensions;

namespace EDO_FOMS.Client.Pages.Personal
{
    public partial class MyOrg
    {
        [Inject] private IOrgManager OrgManager { get; set; }

        private readonly AddEditOrgCommand _orgModel = new();
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

        //public string OrgInn { get; set; }
        public int OrgId { get; set; }

        private ClaimsPrincipal _authUser;
        private bool _canSelfOrgEdit = false;

        private int delay;
        private int duration;

        protected override async Task OnInitializedAsync()
        {
            delay = _stateService.TooltipDelay;
            duration = _stateService.TooltipDuration;

            _authUser = await _authManager.CurrentUser();
            //_authUser = await _authStateProvider.GetAuthenticationStateProviderUserAsync();
            _canSelfOrgEdit = (await _authService.AuthorizeAsync(_authUser, Permissions.SelfOrg.Edit)).Succeeded;

            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            //var state = await _authStateProvider.GetAuthenticationStateAsync();
            //var user = state.User;

            //OrgInn = user.GetInnLe();
            //var response = await OrgManager.GetByInnAsync(OrgInn);

            var response = await OrgManager.GetByIdAsync(new() { Id = Convert.ToInt32(_authUser.GetOrgId()) });
            var org = response.Data;

            await _jsRuntime.InvokeVoidAsync("azino.Console", org);

            if (response.Succeeded)
            {
                _orgModel.Id = org.Id;
                _orgModel.Inn = org.Inn;
                _orgModel.Name = org.Name;
                _orgModel.ShortName = org.ShortName;

                _orgModel.Ogrn = org.Ogrn;
                _orgModel.Email = org.Email;
                _orgModel.Phone = org.Phone;

                _orgModel.IsPublic = org.IsPublic;
                _orgModel.Type = org.Type;
                _orgModel.State = org.State;
            }
        }

        private async Task UpdateOrgAsync()
        {
            var response = await OrgManager.SaveAsync(_orgModel);

            if (response.Succeeded)
            {
                _snackBar.Add(_localizer["Your organization has been updated"], Severity.Success);
            }
        }
    }
}
