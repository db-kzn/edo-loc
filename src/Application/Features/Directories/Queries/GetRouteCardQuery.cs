using AutoMapper;
using EDO_FOMS.Application.Features.Orgs.Queries;
using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Interfaces.Services.Identity;
using EDO_FOMS.Application.Models.Dir;
using EDO_FOMS.Domain.Entities.Dir;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static MudBlazor.CategoryTypes;

namespace EDO_FOMS.Application.Features.Directories.Queries;

public class GetRouteCardQuery : IRequest<Result<RouteCardResponse>>
{
    public int Id { get; }
    //public GetRouteCardQuery() { }
    public GetRouteCardQuery(int id) { Id = id; }
}

internal class GetRouteCardQueryHandler : IRequestHandler<GetRouteCardQuery, Result<RouteCardResponse>>
{
    private readonly IUserService _userService;
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IMapper _mapper;

    public GetRouteCardQueryHandler(
        IUserService userService,
        IUnitOfWork<int> unitOfWork,
        IMapper mapper
        )
    {
        _userService = userService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<RouteCardResponse>> Handle(GetRouteCardQuery request, CancellationToken cancellationToken)
    {
        var routes = _unitOfWork.Repository<Route>().Entities
            .Include(r => r.RouteDocTypes).Include(r => r.ForOrgTypes)
            .Include(r => r.Stages).Include(r => r.Steps).ThenInclude(s => s.Members);

        var route = await routes.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken: cancellationToken);

        var card = new RouteCardResponse
        {
            DocTypeIds = route.RouteDocTypes.Select(dt => dt.DocumentTypeId).ToList(),
            ForOrgTypes = route.ForOrgTypes.Select(ot => ot.OrgType).ToList(),
            Stages = route.Stages.Select(s => new RouteStageModel(s)).ToList(),
            Steps = route.Steps.Where(s => !s.IsDeleted).Select(s => new RouteStepModel(s)).ToList(),

            Parses = new(),

            Id = route.Id,
            Number = route.Number,
            Name = route.Name,
            Description = route.Description,

            ForUserRole = route.ForUserRole,
            EndAction = route.EndAction,

            IsPackage = route.IsPackage,
            CalcHash = route.CalcHash,
            AttachedSign = route.AttachedSign,
            DisplayedSign = route.DisplayedSign,

            IsActive = route.IsActive,
            ReadOnly = route.ReadOnly,
            NameOfFile = route.NameOfFile,
            DateIsToday = route.DateIsToday,

            AllowRevocation = route.AllowRevocation,
            ParseFileName = route.ParseFileName,
            UseVersioning = route.UseVersioning,
            HasDetails = route.HasDetails
        };

        // Вариант .ForEach(async - не работает
        //card.Steps.ForEach(s => s.Members.ForEach(async m => m.Contact = await _userService.GetContactAsync(m.UserId)));
        foreach(var s in card.Steps)
        {
            if (s.OrgId is not null)
            {
                var org = await _unitOfWork.Repository<Organization>().GetByIdAsync((int)s.OrgId);
                s.OrgMember = _mapper.Map<OrgsResponse>(org);// new OrgsResponse();
            }

            foreach(var m in s.Members) m.Contact = await _userService.GetContactAsync(m.UserId);
        }

        return await Result<RouteCardResponse>.SuccessAsync(card);
    }
}
