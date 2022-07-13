using EDO_FOMS.Application.Requests.Directories;
using EDO_FOMS.Shared.Wrapper;
using MediatR;

namespace EDO_FOMS.Application.Features.Directories.Queries;

public class SearchCompaniesQuery : IRequest<PaginatedResult<CompaniesResponse>>
{
    public SearchCompaniesRequest Request { get; }
    public SearchCompaniesQuery(SearchCompaniesRequest request) { Request = request; }
}


