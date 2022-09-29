using AutoMapper;
using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Domain.Entities.Dir;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Constants.Application;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using MudBlazor;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Directories.Commands
{
    public class AddEditDocTypeCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;

        public DocIcons Icon { get; set; } = DocIcons.Undefined;
        public Color Color { get; set; } = Color.Primary;
        public string Label { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;
        public string Short { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public string NameEn { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    internal class AddEditDocTypeCommandHandler : IRequestHandler<AddEditDocTypeCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<AddEditDocTypeCommandHandler> _localizer;

        public AddEditDocTypeCommandHandler(
            IMapper mapper,
            IUnitOfWork<int> unitOfWork,
            IStringLocalizer<AddEditDocTypeCommandHandler> localizer
            )
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(AddEditDocTypeCommand c, CancellationToken ct)
        {
            return (c.Id == 0) ? await AddDocTypeAsync(c, ct) : await EditDocTypeAsync(c, ct);
        }

        private async Task<Result<int>> AddDocTypeAsync(AddEditDocTypeCommand c, CancellationToken cancellationToken)
        {
            var docType = _mapper.Map<DocumentType>(c);
            await _unitOfWork.Repository<DocumentType>().AddAsync(docType);
            await _unitOfWork.Commit(cancellationToken);
            return await Result<int>.SuccessAsync(docType.Id, _localizer["Doc Type Saved"]);
        }

        private async Task<Result<int>> EditDocTypeAsync(AddEditDocTypeCommand command, CancellationToken cancellationToken)
        {
            var docType = await _unitOfWork.Repository<DocumentType>().Entities.FirstOrDefaultAsync(dt => dt.Id == command.Id, cancellationToken);
            if (docType == null) { return await Result<int>.FailAsync(_localizer["Doc Type Not Found!"]); }

            docType.IsActive = command.IsActive;
            
            docType.Icon = command.Icon;
            docType.Color = command.Color;
            docType.Label = command.Label;
            
            docType.Code = command.Code;
            docType.Short = command.Short;
            docType.Name = command.Name;

            docType.NameEn = command.NameEn;
            docType.Description = command.Description;


            await _unitOfWork.Repository<DocumentType>().UpdateAsync(docType);
            await _unitOfWork.Commit(cancellationToken);
            return await Result<int>.SuccessAsync(docType.Id, _localizer["Doc Type Updated"]);
        }
    }
}
