using EDO_FOMS.Application.Interfaces.Services.FileSystem;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Documents.Queries;

public class CheckForImportsQuery : IRequest<Result<int>> { }

internal class CheckForImportsQueryHandler : IRequestHandler<CheckForImportsQuery, Result<int>>
{
    private readonly IDiskService _diskService;
    private readonly IStringLocalizer<CheckForImportsQueryHandler> _localizer;

    public CheckForImportsQueryHandler(
        IDiskService diskService,
        IStringLocalizer<CheckForImportsQueryHandler> localizer
        )
    {
        _diskService = diskService;
        _localizer = localizer;
    }

    public async Task<Result<int>> Handle(CheckForImportsQuery query, CancellationToken cancellationToken)
     {
        var count = _diskService.GetFilesCount("ForDocsImport");

        return await Result<int>.SuccessAsync(count, _localizer["Count of files to import"]);
        //return await Result<int>.FailAsync();
    }
}
