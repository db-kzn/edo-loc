using EDO_FOMS.Application.Requests.Directories;
using EDO_FOMS.Application.Responses.Directories;
using EDO_FOMS.Shared.Wrapper;
using MediatR;

namespace EDO_FOMS.Application.Features.Directories.Queries;

public class SearchDocTypesQuery : IRequest<PaginatedResult<DocTypesResponse>>
{
    public SearchDocTypesRequest Request { get; }
    public SearchDocTypesQuery(SearchDocTypesRequest request) { Request = request; }
}
