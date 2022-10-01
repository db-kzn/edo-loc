using EDO_FOMS.Application.Configurations;
using EDO_FOMS.Application.Interfaces.Services.FileSystem;
using EDO_FOMS.Application.Responses.Directories;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Directories.Queries;

public class CheckCompaniesForImportsQuery : IRequest<Result<CheckCompaniesForImportsResponse>> { }


internal class CheckCompaniesForImportsQueryHandler : IRequestHandler<CheckCompaniesForImportsQuery, Result<CheckCompaniesForImportsResponse>>
{
    private readonly AppStorageInfo _storage;
    //private readonly IDiskService _diskService;
    private readonly IStringLocalizer<CheckCompaniesForImportsQueryHandler> _localizer;

    public CheckCompaniesForImportsQueryHandler(
        IOptions<AppStorageInfo> appStorageInfo,
        //IDiskService diskService,
        IStringLocalizer<CheckCompaniesForImportsQueryHandler> localizer
        )
    {
        _storage = appStorageInfo.Value;
        //_diskService = diskService;
        _localizer = localizer;
    }

    public async Task<Result<CheckCompaniesForImportsResponse>> Handle(CheckCompaniesForImportsQuery request, CancellationToken cancellationToken)
    {
        var path = _storage.PathForImport;

        if (!Directory.Exists(path))
        {
            return await Result<CheckCompaniesForImportsResponse>.FailAsync(_localizer["Companies import folder does not exist"]);
        }

        var response = new CheckCompaniesForImportsResponse()
        {
            Fund = File.Exists(Path.Combine(path, "F001.xml")),
            SMO = File.Exists(Path.Combine(path, "F002.xml")),
            MO = File.Exists(Path.Combine(path, "F003.xml"))
        };

        var result = (response.Fund || response.SMO || response.MO)
            ? _localizer["There are files for importing organizations"]
            : _localizer["There are no files to import organizations"];

        return await Result<CheckCompaniesForImportsResponse>.SuccessAsync(response, result);
    }
}
