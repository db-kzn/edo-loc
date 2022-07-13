using System.Collections.Generic;
using System.Threading.Tasks;
using EDO_FOMS.Application.Features.DocumentTypes.Commands.AddEdit;
using EDO_FOMS.Application.Features.DocumentTypes.Queries.GetAll;
using EDO_FOMS.Shared.Wrapper;

namespace EDO_FOMS.Client.Infrastructure.Managers.Doc.DocumentType
{
    public interface IDocumentTypeManager : IManager
    {
        Task<IResult<List<GetAllDocumentTypesResponse>>> GetAllAsync();

        Task<IResult<int>> SaveAsync(AddEditDocumentTypeCommand request);

        Task<IResult<int>> DeleteAsync(int id);

        Task<IResult<string>> ExportToExcelAsync(string searchString = "");
    }
}