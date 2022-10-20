using AutoMapper;
using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Interfaces.Services.Identity;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using EDO_FOMS.Domain.Entities.Doc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using EDO_FOMS.Application.Responses.Docums;

namespace EDO_FOMS.Application.Features.Documents.Queries;

public class GetDocCardQuery : IRequest<Result<DocCardResponse>>
{
    public int Id { get; }
    public GetDocCardQuery(int id) { Id = id; }
}

internal class GetDocCardQueryHandler : IRequestHandler<GetDocCardQuery, Result<DocCardResponse>>
{
    private readonly IUserService _userService;
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IMapper _mapper;

    public GetDocCardQueryHandler(
        IUserService userService,
        IUnitOfWork<int> unitOfWork,
        IMapper mapper
        )
    {
        _userService = userService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<DocCardResponse>> Handle(GetDocCardQuery request, CancellationToken cancellationToken)
    {
        var docs = _unitOfWork.Repository<Document>().Entities.Include(d => d.Agreements); //.Include(d => d.PacketFiles);//.Include(d => d.Type);

        var doc = await docs.FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken: cancellationToken);

        if (doc is null) { return await Result<DocCardResponse>.FailAsync(); }

        var card = new DocCardResponse()
        {
            Agreements = doc.Agreements.Select(a => new DocCardAgreementResponse(a)).ToList(),

            Id = doc.Id,
            PreviousId = doc.PreviousId,
            ParentId = doc.ParentId,
            RouteId = doc.RouteId,

            EmplId = doc.EmplId,
            EmplOrgId = doc.EmplOrgId,
            ExecutorId = doc.ExecutorId,
            Executor = await _userService.GetContactAsync(doc.ExecutorId),

            IsPublic = doc.IsPublic,
            TypeId = doc.TypeId,
            //TypeName = doc.Type.Name,
            //TypeShort = doc.Type.Short,

            Number = doc.Number,
            Date = doc.Date,
            Title = doc.Title,
            Description = doc.Description,

            //Stage = doc.Stage,
            //CurrentStep = doc.CurrentStep,
            //TotalSteps = doc.TotalSteps,

            URL = doc.URL,
            FileName = doc.FileName
        };

        foreach (var a in card.Agreements) { a.Contact = await _userService.GetContactAsync(a.EmplId); };

        return await Result<DocCardResponse>.SuccessAsync(card);
    }
}
