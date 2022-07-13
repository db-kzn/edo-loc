using EDO_FOMS.Application.Requests.Admin;
using EDO_FOMS.Application.Responses.Identity;
using EDO_FOMS.Client.Extensions;
using EDO_FOMS.Client.Infrastructure.Filters;
using EDO_FOMS.Client.Infrastructure.Managers.System;
using EDO_FOMS.Client.Shared.Dialogs;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Constants.Permission;
using EDO_FOMS.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EDO_FOMS.Client.Pages.Admin
{
    public partial class Users
    {
        [Inject] private IAdminManager AdmManager { get; set; }

        //public Origin TransformOrigin { get; set; } = Origin.TopRight;
        //public Origin AnchorOrigin { get; set; } = Origin.BottomRight;

        private MudTable<UserResponse> _mudTable;
        private IEnumerable<UserResponse> _pagedData;
        private UserResponse _user;
        private readonly List<UserResponse> _userList = new();

        private bool openFilter = true;
        private readonly UserFilter Filter = new();
        private readonly UserFilter FilterDefault = new();

        private bool _loaded;
        private string _searchString = "";
        
        private ClaimsPrincipal _authUser;
        private string userId;
        private bool _canSystemEdit = false;

        private int tz;
        private bool dense;
        private bool matchCase;

        private int delay;
        private int duration;

        private int _totalItems;
        private int _pageNumber = 1;
        private int _rowsPerPage;

        private MudDatePicker _createOnFrom;
        private MudDatePicker _createOnTo;

        protected override async Task OnInitializedAsync()
        {
            _authUser = await _authManager.CurrentUser();
            //_authUser = await _authStateProvider.GetAuthenticationStateProviderUserAsync();
            _canSystemEdit = (await _authService.AuthorizeAsync(_authUser, Permissions.System.Edit)).Succeeded;

            userId = _authUser.GetUserId();
            tz = _stateService.Timezone;

            delay = _stateService.TooltipDelay;
            duration = _stateService.TooltipDuration;

            _rowsPerPage = _stateService.RowsPerPage;
            dense = _stateService.Dense;
            matchCase = _stateService.MatchCase;
            openFilter = _stateService.FilterIsOpen;

            //await GetUsersAsync();
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
            await _mudTable.ReloadServerData();
        }
        private void ToggleFilter()
        {
            openFilter = !openFilter;
            _stateService.FilterIsOpen = openFilter;
        }

        private async Task ApplyFilter()
        {
            await _mudTable.ReloadServerData();
        }
        private async Task RenewAsync()
        {
            FilterReset();
            await _mudTable.ReloadServerData();
            //await GetAgreementsAsync(AgreementStates.AllActive);
        }
        private async Task OnSearch(string text)
        {
            _searchString = text;
            await _mudTable.ReloadServerData();
        }

        private async Task<TableData<UserResponse>> ServerReloadAsync(TableState state = null)
        {
            _loaded = false;

            if (_mudTable.RowsPerPage != 0 && _mudTable.RowsPerPage != _stateService.RowsPerPage)
            {
                _stateService.RowsPerPage = _mudTable.RowsPerPage;
            }

            _rowsPerPage = _stateService.RowsPerPage;
            _pageNumber = (state != null) ? state.Page + 1 : 1;
            await GetUsersAsync(state);

            return new TableData<UserResponse> { TotalItems = _totalItems, Items = _pagedData };
        }
        private async Task GetUsersAsync(TableState state = null)
        {
            string[] orderings = (state != null && !string.IsNullOrEmpty(state.SortLabel))
                ? (orderings = state.SortDirection != SortDirection.None
                    ? new[] { $"{state.SortLabel} {state.SortDirection}" }
                    : new[] { $"{state.SortLabel}" })
                : null;

            PaginatedResult<UserResponse> response;

            if (Filter.IsActive && !Filter.IsEmpty)
            {
                List<OrgTypes> types = new();
                if (!Filter.TypeMO && !Filter.TypeSMO && !Filter.TypeFund)
                {
                    types.AddRange(new List<OrgTypes> { OrgTypes.MO, OrgTypes.SMO, OrgTypes.Fund });
                }
                else
                {
                    if (Filter.TypeMO) { types.Add(OrgTypes.MO); }
                    if (Filter.TypeSMO) { types.Add(OrgTypes.SMO); }
                    if (Filter.TypeFund) { types.Add(OrgTypes.Fund); }
                }

                List<UserBaseRoles> roles = new();
                if (!Filter.RoleUser && !Filter.RoleEmployee && !Filter.RoleManager && !Filter.RoleChief && !Filter.RoleAdmin)
                {
                    roles.AddRange(new List<UserBaseRoles> { UserBaseRoles.User, UserBaseRoles.Employee,
                        UserBaseRoles.Manager, UserBaseRoles.Chief, UserBaseRoles.Admin });
                }
                else
                {
                    if (Filter.RoleUser) { roles.Add(UserBaseRoles.User); }
                    if (Filter.RoleEmployee) { roles.Add(UserBaseRoles.Employee); }
                    if (Filter.RoleManager) { roles.Add(UserBaseRoles.Manager); }
                    if (Filter.RoleChief) { roles.Add(UserBaseRoles.Chief); }
                    if (Filter.RoleAdmin) { roles.Add(UserBaseRoles.Admin); }
                }

                var filter = new SearchUsersRequest()
                {
                    PageSize = _rowsPerPage,
                    PageNumber = _pageNumber,

                    OrderBy = orderings,
                    OrgTypes = types.ToArray(),
                    UserBaseRoles = roles.ToArray(),

                    SearchString = _searchString,
                    MatchCase = matchCase,

                    TextInnLe = Filter.TextInnLe,
                    TextSnils = Filter.TextSnils,
                    
                    TextTitle = Filter.TextTitle,
                    TextSurname = Filter.TextSurname,
                    TextGivenName = Filter.TextGivenName,
                    
                    TextEmail = Filter.TextEmail,
                    TextPhone = Filter.TextPhone,

                    EmailConfirmed = Filter.EmailConfirmed,
                    PhoneConfirmed = Filter.PhoneConfirmed,
                    UserIsActive = Filter.UserIsActive,

                    CreateOnFrom = Filter.CreateOnFrom,
                    CreateOnTo = Filter.CreateOnTo
                };

                await _jsRuntime.InvokeVoidAsync("azino.Console", filter, "Users Filter");
                response = await AdmManager.SearchUsersAsync(filter);
            }
            else
            {
                var request = new GetPagedUsersRequest()
                {
                    PageSize = _rowsPerPage,
                    PageNumber = _pageNumber,
                    SearchString = _searchString,

                    OrderBy = orderings,
                    MatchCase = matchCase
                };

                response = await AdmManager.GetPagedUsersAsync(request);
            }
            //await _jsRuntime.InvokeVoidAsync("azino.Console", response, "Users Search Response");

            if (!response.Succeeded)
            {
                response.Messages.ForEach((m) => _snackBar.Add(m, Severity.Error));
                _loaded = true;
                return;
            }

            _totalItems = response.TotalCount;

            _userList.Clear();
            var data = response.Data;
            data.ToList().ForEach(u =>
            {
                //u.CreatedOn = u.CreatedOn.AddHours(tz);
                _userList.Add(u);
            });

            _pagedData = _userList;
            _loaded = true;
        }

        private async Task AddNewUser()
        {
            var dialog = _dialogService.Show<NewUserDialog>(_localizer["New User"]);
            var result = await dialog.Result;

            if (!result.Cancelled)
            {
                //await GetUsersAsync();
                await _mudTable.ReloadServerData();
            }
        }
        async Task EditUser(TableRowClickEventArgs<UserResponse> e)
        {
            if (e is null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            var ops = new DialogParameters
            {
                {
                    nameof(UserEditDialog.EditUser),
                    new EditUserRequest
                    {
                        Id = _user.Id,
                        InnLe = _user.InnLe,
                        Snils = _user.Snils,
                        Inn = _user.Inn,

                        Title = _user.Title,
                        Surname = _user.Surname,
                        GivenName = _user.GivenName,

                        OrgType = _user.OrgType,
                        BaseRole = _user.BaseRole,

                        IsActive = _user.IsActive,
                        Email = _user.Email,
                        EmailConfirmed = _user.EmailConfirmed,
                        PhoneNumber = _user.PhoneNumber,
                        PhoneNumberConfirmed = _user.PhoneNumberConfirmed
                    }
                }
            };

            var dialog = _dialogService.Show<UserEditDialog>("", ops);
            var result = await dialog.Result;

            if (!result.Cancelled) {
                //await GetUsersAsync();
                await _mudTable.ReloadServerData();
            }
        }

        private void OnText()
        {
            Filter.ChangedText = (Filter.TextInnLe != FilterDefault.TextInnLe
                || Filter.TextSnils != FilterDefault.TextSnils
                || Filter.TextTitle != FilterDefault.TextTitle
                || Filter.TextSurname != FilterDefault.TextSurname
                || Filter.TextGivenName != FilterDefault.TextGivenName
                || Filter.TextEmail != FilterDefault.TextEmail
                || Filter.TextPhone != FilterDefault.TextPhone);

            FilterIsActive();
        }
        private void OnState(string state)
        {
            if (state == "EmailConfirmed")
            {
                Filter.EmailConfirmed = (Filter.EmailConfirmed == null) ? true
                    : Filter.EmailConfirmed == true ? false : null;
            }
            else if (state == "PhoneConfirmed")
            {
                Filter.PhoneConfirmed = (Filter.PhoneConfirmed == null) ? true
                    : Filter.PhoneConfirmed == true ? false : null;
            }
            else if (state == "UserIsActive")
            {
                Filter.UserIsActive = (Filter.UserIsActive == null) ? true
                    : Filter.UserIsActive == true ? false : null;
            }

            Filter.ChangedStates = (Filter.EmailConfirmed != FilterDefault.EmailConfirmed
                || Filter.PhoneConfirmed != FilterDefault.PhoneConfirmed
                || Filter.UserIsActive != FilterDefault.UserIsActive);

            FilterIsActive();
        }
        private void OnType(string type)
        {
            if (type == "MO") { Filter.TypeMO = !Filter.TypeMO; }
            else if (type == "SMO") { Filter.TypeSMO = !Filter.TypeSMO; }
            else if (type == "Fund") { Filter.TypeFund = !Filter.TypeFund; }

            Filter.ChangedTypes = (Filter.TypeMO != FilterDefault.TypeMO
                || Filter.TypeSMO != FilterDefault.TypeSMO
                || Filter.TypeFund != FilterDefault.TypeFund);

            FilterIsActive();
            //FilterIsEmpty();
        }
        private void OnRole(string role)
        {
            if (role == "User") { Filter.RoleUser = !Filter.RoleUser; }
            else if (role == "Employee") { Filter.RoleEmployee = !Filter.RoleEmployee; }
            else if (role == "Manager") { Filter.RoleManager = !Filter.RoleManager; }
            else if (role == "Chief") { Filter.RoleChief = !Filter.RoleChief; }
            else if (role == "Admin") { Filter.RoleAdmin = !Filter.RoleAdmin; }

            Filter.ChangedRoles = (Filter.RoleUser != FilterDefault.RoleUser
                || Filter.RoleEmployee != FilterDefault.RoleEmployee
                || Filter.RoleManager != FilterDefault.RoleManager
                || Filter.RoleChief != FilterDefault.RoleChief
                || Filter.RoleAdmin != FilterDefault.RoleAdmin);

            FilterIsActive();
            //FilterIsEmpty();
        }
        private void OnCreateOn(string c, DateTime? d)
        {
            if (c == "From") { Filter.CreateOnFrom = d; }
            else if (c == "To") { Filter.CreateOnTo = d; }

            Filter.ChangedCreateOn = Filter.CreateOnFrom != FilterDefault.CreateOnFrom ||
                Filter.CreateOnTo != FilterDefault.CreateOnTo;

            FilterIsActive();
        }

        private void FilterIsActive()
        {
            Filter.IsActive = Filter.ChangedText || Filter.ChangedStates || Filter.ChangedTypes || Filter.ChangedRoles || Filter.ChangedCreateOn;
        }
        //private void FilterIsEmpty()
        //{
        //    Filter.IsEmpty = !(Filter.RoleUser || Filter.RoleEmployee || Filter.RoleManager || Filter.RoleChief || Filter.RoleAdmin)
        //        || !(Filter.TypeMO || Filter.TypeSMO || Filter.TypeFund);
        //}
        private void FilterReset()
        {
            Filter.TextInnLe = FilterDefault.TextInnLe;
            Filter.TextSnils = FilterDefault.TextSnils;

            Filter.TextTitle = FilterDefault.TextTitle;
            Filter.TextSurname = FilterDefault.TextSurname;
            Filter.TextGivenName = FilterDefault.TextGivenName;

            Filter.TextEmail = FilterDefault.TextEmail;
            Filter.TextPhone = FilterDefault.TextPhone;

            Filter.EmailConfirmed = FilterDefault.EmailConfirmed;
            Filter.PhoneConfirmed = FilterDefault.PhoneConfirmed;
            Filter.UserIsActive = FilterDefault.UserIsActive;

            Filter.TypeMO = FilterDefault.TypeMO;
            Filter.TypeSMO = FilterDefault.TypeSMO;
            Filter.TypeFund = FilterDefault.TypeFund;

            Filter.RoleUser = FilterDefault.RoleUser;
            Filter.RoleEmployee = FilterDefault.RoleEmployee;
            Filter.RoleManager = FilterDefault.RoleManager;
            Filter.RoleChief = FilterDefault.RoleChief;
            Filter.RoleAdmin = FilterDefault.RoleAdmin;

            Filter.CreateOnFrom = FilterDefault.CreateOnFrom;
            Filter.CreateOnTo = FilterDefault.CreateOnTo;

            Filter.ChangedText = false;
            Filter.ChangedStates = false;
            Filter.ChangedTypes = false;
            Filter.ChangedRoles = false;
            Filter.ChangedCreateOn = false;

            Filter.IsActive = false;
            Filter.IsEmpty = false;
        }

        //private bool Search(UserResponse user)
        //{
        //    if (string.IsNullOrWhiteSpace(_searchString)) return true;
        //    var comparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        //    return user.InnLe?.Contains(_searchString, comparison) == true ||
        //        user.Snils?.Contains(_searchString, comparison) == true ||
        //        user.Title?.Contains(_searchString, comparison) == true ||
        //        user.Surname?.Contains(_searchString, comparison) == true ||
        //        user.GivenName?.Contains(_searchString, comparison) == true ||
        //        user.Email?.Contains(_searchString, comparison) == true;
        //}

        //private async Task ExportToExcel()
        //{
        //    var base64 = await _userManager.ExportToExcelAsync(_searchString);
        //    await _jsRuntime.InvokeVoidAsync("Download", new
        //    {
        //        ByteArray = base64,
        //        FileName = $"{nameof(Users).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
        //        MimeType = AppConstants.MimeTypes.OpenXml
        //    });
        //    _snackBar.Add(string.IsNullOrWhiteSpace(_searchString)
        //        ? _localizer["Users exported"]
        //        : _localizer["Filtered Users exported"], Severity.Success);
        //}

        //private async Task InvokeModal()
        //{
        //    var parameters = new DialogParameters();
        //    var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
        //    var dialog = _dialogService.Show<NewUserDialog>(_localizer["Register New User"], parameters, options);
        //    var result = await dialog.Result;

        //    if (!result.Cancelled)
        //    {
        //        await GetUsersAsync();
        //    }
        //}

        //private void ViewProfile(string userId)
        //{
        //    _navigationManager.NavigateTo($"/user-profile/{userId}");
        //}

        //private void ManageRoles(string userId, string email)
        //{
        //    if (email == "super@admin")
        //    {
        //        _snackBar.Add(_localizer["Not Allowed."], Severity.Error);
        //    }
        //    else
        //    {
        //        _navigationManager.NavigateTo($"/identity/user-roles/{userId}");
        //    }
        //}
    }
}
