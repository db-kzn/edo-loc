using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Interfaces.Services.FileSystem;
using EDO_FOMS.Application.Models.Dir;
using EDO_FOMS.Domain.Entities.Dir;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Documents.Queries;

public class CheckForImportsQuery : IRequest<Result<List<ActiveRouteModel>>> { }

internal class CheckForImportsQueryHandler : IRequestHandler<CheckForImportsQuery, Result<List<ActiveRouteModel>>>
{
    private readonly IDiskService _diskService;
    private readonly IStringLocalizer<CheckForImportsQueryHandler> _localizer;
    private readonly IUnitOfWork<int> _unitOfWork;

    public CheckForImportsQueryHandler(
        IDiskService diskService,
        IStringLocalizer<CheckForImportsQueryHandler> localizer,
        IUnitOfWork<int> unitOfWork
        )
    {
        _diskService = diskService;
        _localizer = localizer;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<ActiveRouteModel>>> Handle(CheckForImportsQuery query, CancellationToken cancellationToken)
     {
        Expression<Func<Route, ActiveRouteModel>> expression = e => new ActiveRouteModel
        {
            Id = e.Id,
            Number = e.Number,
            Count = null,

            Code = e.Code,
            Short = e.Short,
            Name = e.Name,

            Description = e.Description,
            ParseFileName = e.ParseFileName,
            Mask = e.Parses
                    .Where(p => p.PatternType == ParsePatterns.Mask)
                    .Select(p => p.Pattern)
                    .FirstOrDefault()
        };

        var activeRoutes = _unitOfWork.Repository<Route>().Entities
            .Where(r => r.IsActive)
            .Select(expression)
            .ToList(); ;

        //var count = _diskService.GetFilesCount("ForDocsImport");
        activeRoutes.ForEach(r =>
        {
            r.Count = _diskService.GetFilesCount("ForDocsImport", r.Mask);
        });

        return await Result<List<ActiveRouteModel>>.SuccessAsync(activeRoutes, _localizer["Imports Count"]);
    }
}
