using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Responses.Directories;
using EDO_FOMS.Domain.Entities.Dir;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Directories.Queries;

public class GetAllDocTypeTitlesQuery : IRequest<Result<List<GetAllDocTypeTitlesResponse>>>
{
    public GetAllDocTypeTitlesQuery() { }
}

internal class GetAllDocTypeTitlesQueryHandler : IRequestHandler<GetAllDocTypeTitlesQuery, Result<List<GetAllDocTypeTitlesResponse>>>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    public GetAllDocTypeTitlesQueryHandler(IUnitOfWork<int> unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<Result<List<GetAllDocTypeTitlesResponse>>> Handle(GetAllDocTypeTitlesQuery _, CancellationToken cancellationToken)
    {
        var docTypeTitles = await _unitOfWork.Repository<DocumentType>().Entities
            .Where(dt => dt.IsActive)
            .Select(dt => new GetAllDocTypeTitlesResponse()
            {
                Id = dt.Id,
                Icon = dt.Icon,
                Color = dt.Color,
                Label = dt.Label
            })
            .ToListAsync(cancellationToken);

        return await Result<List<GetAllDocTypeTitlesResponse>>.SuccessAsync(docTypeTitles);
    }
}
