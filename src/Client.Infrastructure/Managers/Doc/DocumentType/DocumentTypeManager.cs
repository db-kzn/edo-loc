using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EDO_FOMS.Application.Features.DocumentTypes.Commands.AddEdit;
using EDO_FOMS.Application.Features.DocumentTypes.Queries;
using EDO_FOMS.Client.Infrastructure.Extensions;
using EDO_FOMS.Shared.Wrapper;

namespace EDO_FOMS.Client.Infrastructure.Managers.Doc.DocumentType
{
    public class DocumentTypeManager : IDocumentTypeManager
    {
        private readonly HttpClient _httpClient;

        public DocumentTypeManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<string>> ExportToExcelAsync(string searchString = "")
        {
            var response = await _httpClient.GetAsync(string.IsNullOrWhiteSpace(searchString)
                ? Routes.DocumentTypesEndpoints.Export
                : Routes.DocumentTypesEndpoints.ExportFiltered(searchString));
            return await response.ToResult<string>();
        }

        public async Task<IResult<int>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{Routes.DocumentTypesEndpoints.Delete}/{id}");
            return await response.ToResult<int>();
        }

        public async Task<IResult<List<DocTypeResponse>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(Routes.DocumentTypesEndpoints.GetAll);
            return await response.ToResult<List<DocTypeResponse>>();
        }

        public async Task<IResult<int>> SaveAsync(AddEditDocumentTypeCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.DocumentTypesEndpoints.Save, request);
            return await response.ToResult<int>();
        }
    }
}