using AutoMapper;
using EDO_FOMS.Application.Features.Orgs.Queries;
using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Interfaces.Services.Identity;
using EDO_FOMS.Application.Models.Dir;
using EDO_FOMS.Application.Responses.Directories;
using EDO_FOMS.Domain.Entities.Dir;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            .Include(r => r.RouteDocTypes).Include(r => r.ForOrgTypes).Include(r => r.Parses)
            .Include(r => r.Stages).Include(r => r.Steps).ThenInclude(s => s.Members);

        var route = await routes.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken: cancellationToken);

        if (route is null) { return await Result<RouteCardResponse>.FailAsync(); }

        var card = new RouteCardResponse
        {
            DocTypeIds = route.RouteDocTypes.Select(dt => dt.DocumentTypeId).ToList(),
            ForOrgTypes = route.ForOrgTypes.Select(ot => ot.OrgType).ToList(),

            Stages = route.Stages.Select(s => new RouteStageModel(s)).ToList(),
            Steps = route.Steps.Where(s => !s.IsDeleted).Select(s => new RouteStepModel(s)).ToList(),
            Parses = route.Parses.Select(s => new RouteFileParseModel(s)).ToList(),
            Files = null, // Нужно реализовать, пока нет пакетов файлов

            Id = route.Id,
            Number = route.Number,
            Code = route.Code,

            Short = route.Short,
            Name = route.Name,
            Description = route.Description,

            ExecutorId = route.ExecutorId,
            Executor = await _userService.GetContactAsync(route.ExecutorId),
            ForUserRole = route.ForUserRole,
            EndAction = route.EndAction,

            IsActive = route.IsActive,
            DateIsToday = route.DateIsToday,
            NameOfFile = route.NameOfFile,
            ParseFileName = route.ParseFileName,

            AllowRevocation = route.AllowRevocation,
            ProtectedMode = route.ProtectedMode,
            ShowNotes = route.ShowNotes,
            UseVersioning = route.UseVersioning,

            IsPackage = route.IsPackage,
            CalcHash = route.CalcHash,
            AttachedSign = route.AttachedSign,
            DisplayedSign = route.DisplayedSign,

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

            foreach (var m in s.Members) { m.Contact = await _userService.GetContactAsync(m.UserId); }
        }

        return await Result<RouteCardResponse>.SuccessAsync(card);
    }
}
