using EDO_FOMS.Application.Extensions;
using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Requests.Directories;
using EDO_FOMS.Application.Responses.Directories;
using EDO_FOMS.Application.Specifications.Dir;
using EDO_FOMS.Domain.Entities.Dir;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Directories.Queries;

public class GetCompaniesQuery : IRequest<PaginatedResult<CompaniesResponse>>
{
    public GetPagedCompaniesRequest Request { get; set; }
    public GetCompaniesQuery(GetPagedCompaniesRequest request)
    {
        Request = request;
    }
}

internal class GetCompaniesQueryHandler : IRequestHandler<GetCompaniesQuery, PaginatedResult<CompaniesResponse>>
{
    private readonly IUnitOfWork<int> _unitOfWork;

    public GetCompaniesQueryHandler(IUnitOfWork<int> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResult<CompaniesResponse>> Handle(GetCompaniesQuery query, CancellationToken cancellationToken)
    {
        var request = query.Request;

        Expression<Func<Company, CompaniesResponse>> expression = e => new CompaniesResponse
        {
            Id = e.Id,
            Type = e.Type,
            State = e.State,
            TfOkato = e.TfOkato,

            Code = e.Code,
            Inn = e.Inn,
            Kpp = e.Kpp,
            Ogrn = e.Ogrn,

            Name = e.Name,
            ShortName = e.ShortName,
            Address = e.Address,
            AO = e.AO,

            Phone = e.Phone,
            Fax = e.Fax,
            HotLine = e.HotLine,
            Email = e.Email,
            SiteUrl = e.SiteUrl,

            HeadName = e.HeadName,
            HeadLastName = e.HeadLastName,
            HeadMidName = e.HeadMidName,
            Changed = e.Changed
        };

        var companySpec = new CompanyFilterSpecification(request.SearchString, request.MatchCase);

        var sort = (request.OrderBy?.Any() == true) ? string.Join(",", request.OrderBy) : "Id Descending";

        return await _unitOfWork.Repository<Company>().Entities
           .Specify(companySpec)
           .Select(expression)
           .OrderBy(sort)
           .ToPaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
