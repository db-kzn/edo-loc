using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Interfaces.Services.FileSystem;
using EDO_FOMS.Domain.Entities.Dir;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Documents.Queries;

public class GetImportFilesQuery : IRequest<Result<List<string>>>
{
    public int RouteId { get; }
    public GetImportFilesQuery(int routeId) { RouteId = routeId; }
}

internal class GetImportFilesQueryHandler : IRequestHandler<GetImportFilesQuery, Result<List<string>>>
{
    private readonly IDiskService _diskService;
    private readonly IStringLocalizer<GetImportFilesQueryHandler> _localizer;
    private readonly IUnitOfWork<int> _unitOfWork;

    public GetImportFilesQueryHandler(
        IDiskService diskService,
        IStringLocalizer<GetImportFilesQueryHandler> localizer,
        IUnitOfWork<int> unitOfWork
        )
    {
        _diskService = diskService;
        _localizer = localizer;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<string>>> Handle(GetImportFilesQuery request, CancellationToken cancellationToken)
    {
        var route = await _unitOfWork.Repository<Route>().Entities.Include(r => r.Parses).FirstOrDefaultAsync(r => r.Id == request.RouteId, cancellationToken);

        if (route is null) { return await Result<List<string>>.FailAsync(_localizer["Route not found"]); }

        var mask = route.Parses
                        .Where(p => p.PatternType == ParsePatterns.Mask)
                        .Select(p => p.Pattern)
                        .FirstOrDefault();

        var files = _diskService.GetImportFiles(mask);

        return await Result<List<string>>.SuccessAsync(files, _localizer["Files available for import are received"]);
    }
}
