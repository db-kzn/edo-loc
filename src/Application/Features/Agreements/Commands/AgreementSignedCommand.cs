using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Interfaces.Services.FileSystem;
using EDO_FOMS.Application.Interfaces.Services.Identity;
using EDO_FOMS.Domain.Entities.Doc;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Agreements.Commands
{
    public class AgreementSignedCommand : IRequest<Result<int>>
    {
        public int AgreementId { get; set; }
        public string EmplId { get; set; }
        public int EmplOrgId { get; set; }
        public int DocId { get; set; }
        //public string Thumbprint { get; set; }

        public byte[] Data { get; set; }
    }

    internal class AgreementSignedCommandHandler : IRequestHandler<AgreementSignedCommand, Result<int>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IUserService _userService;
        private readonly IUploadService _uploadService;
        private readonly IStringLocalizer<AgreementSignedCommandHandler> _localizer;

        public AgreementSignedCommandHandler(
            IUnitOfWork<int> unitOfWork,
            IUserService userService,
            IUploadService uploadService,
            IStringLocalizer<AgreementSignedCommandHandler> localizer
            )
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _uploadService = uploadService;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(AgreementSignedCommand command, CancellationToken cancellationToken)
        {
            // Согласование для которого принимается ответ
            var agreement = await _unitOfWork.Repository<Agreement>().GetByIdAsync(command.AgreementId);

            //agreement.State = AgreementStates.Signed;
            //agreement.Remark = command.Remark;
            //agreement.Answered = DateTime.Now;

            var doc = await _unitOfWork.Repository<Document>().GetByIdAsync(agreement.DocumentId);
            if (doc == null) { return await Result<int>.FailAsync(_localizer["Document not found"]); }

            var org = await _unitOfWork.Repository<Organization>().GetByIdAsync(agreement.OrgId);
            if (org == null) { return await Result<int>.FailAsync(_localizer["Organization not found"]); }

            var employee = _userService.GetEmployeeAsync(agreement.EmplId).Result;
            if (employee == null) { return await Result<int>.FailAsync(_localizer["Employee not found"]); }

            var initials = new string(Array.ConvertAll(
                employee.GivenName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries),
                n => n[0]));

            var orgName = !string.IsNullOrWhiteSpace(org.ShortName) ? org.ShortName 
                : org.Name[..((org.Name.Length > 32) ? 32 : org.Name.Length)];

            orgName = orgName.Replace("\"", "").Replace(" ", "_");

            var signName = $"{org.Inn}_{employee.Snils}_{employee.Surname}{initials}_{orgName}";

            var result = _uploadService.UploadSign(command.Data, doc.StoragePath, signName);

            if (!result) { return await Result<int>.FailAsync(_localizer["Signing failed"]); }

            return await Result<int>.SuccessAsync(agreement.Id, $"{_localizer["Agreement Signed"]}: {signName}");
        }
    }
}
