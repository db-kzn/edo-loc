using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Domain.Entities.Dir;
using EDO_FOMS.Shared.Wrapper;
using MediatR;

namespace EDO_FOMS.Application.Features.DocumentTypes.Queries.GetById
{
    public class GetDocumentTypeByIdQuery : IRequest<Result<DocTypeResponse>>
    {
        public int Id { get; set; }
    }

    internal class GetDocumentTypeByIdQueryHandler : IRequestHandler<GetDocumentTypeByIdQuery, Result<DocTypeResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetDocumentTypeByIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<DocTypeResponse>> Handle(GetDocumentTypeByIdQuery query, CancellationToken cancellationToken)
        {
            var documentType = await _unitOfWork.Repository<DocumentType>().GetByIdAsync(query.Id);
            var mappedDocumentType = _mapper.Map<DocTypeResponse>(documentType);
            return await Result<DocTypeResponse>.SuccessAsync(mappedDocumentType);
        }
    }
}