using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EDO_FOMS.Application.Features.DocumentTypes.Commands.AddEdit;
using EDO_FOMS.Application.Features.DocumentTypes.Queries;
using EDO_FOMS.Client.Extensions;
using EDO_FOMS.Client.Infrastructure.Managers.Doc.DocumentType;
using EDO_FOMS.Shared.Constants.Application;
using EDO_FOMS.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;

namespace EDO_FOMS.Client.Pages.Docs
{
    public partial class DocumentTypes
    {
        [Inject] private IDocumentTypeManager DocumentTypeManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private List<DocTypeResponse> _documentTypeList = new();
        private DocTypeResponse _documentType = new();
        private string _searchString = "";
        private bool _dense = false;
        private bool _striped = true;
        private bool _bordered = false;

        private ClaimsPrincipal _currentUser;
        private bool _canCreateDocumentTypes;
        private bool _canEditDocumentTypes;
        private bool _canDeleteDocumentTypes;
        private bool _canExportDocumentTypes;
        private bool _canSearchDocumentTypes;
        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authManager.CurrentUser();
            _canCreateDocumentTypes = (await _authService.AuthorizeAsync(_currentUser, Permissions.DocumentTypes.Create)).Succeeded;
            _canEditDocumentTypes = (await _authService.AuthorizeAsync(_currentUser, Permissions.DocumentTypes.Edit)).Succeeded;
            _canDeleteDocumentTypes = (await _authService.AuthorizeAsync(_currentUser, Permissions.DocumentTypes.Delete)).Succeeded;
            _canExportDocumentTypes = (await _authService.AuthorizeAsync(_currentUser, Permissions.DocumentTypes.Export)).Succeeded;
            _canSearchDocumentTypes = (await _authService.AuthorizeAsync(_currentUser, Permissions.DocumentTypes.Search)).Succeeded;

            await GetDocumentTypesAsync();
            _loaded = true;
            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        private async Task GetDocumentTypesAsync()
        {
            var response = await DocumentTypeManager.GetAllAsync();
            if (response.Succeeded)
            {
                _documentTypeList = response.Data.ToList();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private async Task Delete(int id)
        {
            string deleteContent = _localizer["Delete Content"];
            var parameters = new DialogParameters
            {
                {nameof(Shared.Dialogs.DeleteConfirmation.ContentText), string.Format(deleteContent, id)}
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<Shared.Dialogs.DeleteConfirmation>(_localizer["Delete"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                var response = await DocumentTypeManager.DeleteAsync(id);
                if (response.Succeeded)
                {
                    await Reset();
                    await HubConnection.SendAsync(AppConstants.SignalR.SendUpdateDashboard);
                    _snackBar.Add(response.Messages[0], Severity.Success);
                }
                else
                {
                    await Reset();
                    foreach (var message in response.Messages)
                    {
                        _snackBar.Add(message, Severity.Error);
                    }
                }
            }
        }

        private async Task ExportToExcel()
        {
            var response = await DocumentTypeManager.ExportToExcelAsync(_searchString);
            if (response.Succeeded)
            {
                await _jsRuntime.InvokeVoidAsync("azino.Download", new
                {
                    ByteArray = response.Data,
                    FileName = $"{nameof(DocumentTypes).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                    MimeType = AppConstants.MimeTypes.OpenXml
                });
                _snackBar.Add(string.IsNullOrWhiteSpace(_searchString)
                    ? _localizer["Document Types exported"]
                    : _localizer["Filtered Document Types exported"], Severity.Success);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private async Task InvokeModal(int id = 0)
        {
            var parameters = new DialogParameters();
            if (id != 0)
            {
                _documentType = _documentTypeList.FirstOrDefault(c => c.Id == id);
                if (_documentType != null)
                {
                    parameters.Add(nameof(AddEditDocumentTypeModal.AddEditDocumentTypeModel), new AddEditDocumentTypeCommand
                    {
                        Id = _documentType.Id,
                        Name = _documentType.Name,
                        Description = _documentType.Description
                    });
                }
            }
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<AddEditDocumentTypeModal>(id == 0 ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await Reset();
            }
        }

        private async Task Reset()
        {
            _documentType = new DocTypeResponse();
            await GetDocumentTypesAsync();
        }

        private bool Search(DocTypeResponse brand)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;
            if (brand.Name?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            if (brand.Description?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            return false;
        }
    }
}
