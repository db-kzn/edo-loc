using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Interfaces.Services.Identity;
using EDO_FOMS.Application.Models;
using EDO_FOMS.Domain.Entities.Doc;
using EDO_FOMS.Domain.Entities.Public;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Agreements.Commands
{
    public partial class AddAgreementMembersCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public List<string> Members { get; set; }
    }

    internal class AddAgreementMembersCommandHandler : IRequestHandler<AddAgreementMembersCommand, Result<int>>
    {
        private readonly IUserService _userService;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<AddAgreementMembersCommandHandler> _localizer;

        public AddAgreementMembersCommandHandler(
            IUserService userService,
            IUnitOfWork<int> unitOfWork,
            IStringLocalizer<AddAgreementMembersCommandHandler> localizer
            )
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(AddAgreementMembersCommand command, CancellationToken cancellationToken)
        {
            if (command.Members.Count == 0) { return await Result<int>.FailAsync(_localizer["No members to add"]); }

            var agreement = await _unitOfWork.Repository<Agreement>().GetByIdAsync(command.Id);

            if (agreement == null) { return await Result<int>.FailAsync(_localizer["Agreement not found"]); }

            //List<Agreement> agreements = new();
            var doc = await _unitOfWork.Repository<Document>().GetByIdAsync(agreement.DocumentId);

            List<MailToUser> mails = new();

            command.Members.ForEach(async emplId =>
            {
                Agreement a = new()
                {
                    DocumentId = agreement.DocumentId,
                    EmplId = emplId,
                    OrgId = agreement.OrgId,
                    ParentId = command.Id,

                    StageNumber = agreement.StageNumber,
                    State = AgreementStates.Incoming,
                    Action = ActTypes.Review,//AgreementActions.ToReview,
                    IsCanceled = agreement.IsCanceled

                    // Remark // Time // URL
                };

                var subscribe = _unitOfWork.Repository<Subscribe>().Entities.FirstOrDefault(s => s.UserId == emplId && s.Email.AgreementIncoming); // 1

                if (subscribe is not null)
                {
                    var mail = new MailToUser()
                    {
                        UserId = emplId,
                        Theme = _localizer["New document received"],
                        Text = $"{_localizer["Dear user!"]}<br/><br/>{_localizer["You have received a new document for signing (approval) {0} of {1:d} {2}", doc.Number, doc.Date, doc.Title]}"
                    };

                    mails.Add(mail);
                    //await _userService.MailToUserAsync(mail);
                }

                await _unitOfWork.Repository<Agreement>().AddAsync(a);
            });

            await _unitOfWork.Commit(cancellationToken);
            await _userService.MailsToUsersAsync(mails);

            return await Result<int>.SuccessAsync(command.Id, _localizer["Members added"]);
        }
    }
}
