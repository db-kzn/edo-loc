using EDO_FOMS.Application.Requests.Person;
using EDO_FOMS.Application.Responses.Identity;
using EDO_FOMS.Client.Extensions;
using EDO_FOMS.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Pages.Personal
{
    public partial class Employees
    {
        private MudTable<UserResponse> mudTable;
        private readonly List<UserResponse> _employeeList = new();
        private UserResponse _employee = new();

        private bool _loaded = false;
        private string _searchString = "";

        private ClaimsPrincipal _authUser;
        private bool _canSelfOrgEdit = false;
        
        private int tz;
        private bool dense;
        private bool matchCase = false;

        private int delay;
        private int duration;

        protected override async Task OnInitializedAsync()
        {
            _authUser = await _authManager.CurrentUser();
            _canSelfOrgEdit = (await _authService.AuthorizeAsync(_authUser, Permissions.SelfOrg.Edit)).Succeeded;

            tz = _stateService.Timezone;
            dense = _stateService.Dense;
            matchCase = _stateService.MatchCase;

            delay = _stateService.TooltipDelay;
            duration = _stateService.TooltipDuration;

            await GetEmployeesAsync();
        }

        private void OnToggledDense(bool toggled)
        {
            dense = toggled;
            _stateService.Dense = dense;
        }
        private async Task OnToggledMatchCase(bool toggled)
        {
            matchCase = toggled;
            _stateService.MatchCase = matchCase;
            //await _mudTable.ReloadServerData();
            await GetEmployeesAsync();
        }

        private async Task RenewAsync() => await GetEmployeesAsync();

        private async Task GetEmployeesAsync()
        {
            _loaded = false;

            var orgId = Convert.ToInt32(_authUser.GetOrgId());
            var response = await _userManager.GetAllByOrgIdAsync(orgId);

            if (response.Succeeded)
            {
                _employeeList.Clear();

                //_employeeList.AddRange(response.Data.ToList());
                response.Data.ToList().ForEach(e =>
                {
                    //e.CreatedOn = e.CreatedOn.AddHours(tz);
                    _employeeList.Add(e);
                });
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }

            _loaded = true;
        }

        private async Task Reset()
        {
            _employee = new();
            await GetEmployeesAsync();
        }

        //TableRowClickEventArgs<UserResponse> e
        //if (e is null) { throw new ArgumentNullException(nameof(e)); }
        async Task EditEmployee() => await AddEditEmployee(_employee);

        async Task AddNewEmployee() => await AddEditEmployee(new());

        async Task AddEditEmployee(UserResponse employee)
        {
            var param = new DialogParameters
            {
                {
                    nameof(EmployeeDialog.EmployeeModel),
                    new AddEditEmployeeRequest
                    {
                        Id = employee.Id,
                        Title = employee.Title,

                        Surname = employee.Surname,
                        GivenName = employee.GivenName,

                        Snils = employee.Snils,
                        Inn = employee.Inn,

                        Email = employee.Email,
                        PhoneNumber = employee.PhoneNumber,

                        BaseRole = employee.BaseRole,
                        IsActive = employee.IsActive
                    }
                }
            };

            var dialog = _dialogService.Show<EmployeeDialog>("", param);
            var result = await dialog.Result;

            if (!result.Cancelled) { await Reset(); }
        }

        private bool Search(UserResponse user)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;

            return matchCase ? (
                user.InnLe?.Contains(_searchString) == true ||
                user.Snils?.Contains(_searchString) == true ||
                user.Title?.Contains(_searchString) == true ||
                user.Surname?.Contains(_searchString) == true ||
                user.GivenName?.Contains(_searchString) == true ||
                user.Email?.Contains(_searchString) == true
                //user.PhoneNumber?.Contains(_searchString) == true ||
                //user.UserName?.Contains(_searchString) == true
                ) : (
                user.InnLe?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                user.Snils?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                user.Title?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                user.Surname?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                user.GivenName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                user.Email?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true
                );
        }
    }
}
