using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Interfaces.Services;
using EDO_FOMS.Application.Responses.Agreements;
using EDO_FOMS.Domain.Entities.Doc;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Agreements.Queries
{
    public class EmployeeAgreementsQuery : IRequest<Result<List<EmployeeAgreementsResponse>>>
    {
        //public string UserId { get; set; }
        public AgreementStates State { get; set; }
        public bool IsCanceled { get; set; }

        public EmployeeAgreementsQuery(
            //string userId,
            AgreementStates state,
            bool isCanceled = false
            )
        {
            //UserId = userId;
            State = state;
            IsCanceled = isCanceled;
        }
    }

    internal class EmployeeAgreementsQueryHandler : IRequestHandler<EmployeeAgreementsQuery, Result<List<EmployeeAgreementsResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly string _emplId;

        //private readonly IStringLocalizer<EmployeeAgreementsQueryHandler> _localizer;
        //private readonly IUserService _userService;
        //private readonly IMapper _mapper;
        //private readonly IAppCache _cache;

        public EmployeeAgreementsQueryHandler(
            IUnitOfWork<int> unitOfWork,
            ICurrentUserService currentUserService
            //IStringLocalizer<EmployeeAgreementsQueryHandler> localizer
            //IUserService userService
            //IMapper mapper
            //IAppCache cache
            )
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _emplId = _currentUserService.UserId;

            //_localizer = localizer;
            //_userService = userService;
            //_mapper = mapper;
            //_cache = cache;
        }


        public async Task<Result<List<EmployeeAgreementsResponse>>> Handle(EmployeeAgreementsQuery request, CancellationToken cancellationToken)
        {
            var employeeAgreements = _unitOfWork.Repository<Agreement>().Entities.Where(a => a.EmplId == _emplId);

            IQueryable<Agreement> query;
            if (request.State == AgreementStates.AllActive)
            {
                query = employeeAgreements.Where(a => a.Document.Stage == DocStages.InProgress
                    && a.State != AgreementStates.Control && a.State != AgreementStates.Expecting);
                    //(a.State == AgreementStates.Incoming || a.State == AgreementStates.Received || a.State == AgreementStates.Opened ||
                    //a.State == AgreementStates.Verifed || a.State == AgreementStates.Approved || a.State == AgreementStates.Signed ||
                    //a.State == AgreementStates.Refused || a.State == AgreementStates.Rejected) && a.Document.Stage != DocStages.Archive);
            }
            else if (request.State == AgreementStates.Approved)
            {
                query = employeeAgreements.Where(a => a.State == AgreementStates.Verifed || a.State == AgreementStates.Approved || a.State == AgreementStates.Signed);
            }
            else if (request.State == AgreementStates.Incoming)
            {
                query = employeeAgreements.Where(a => a.State == AgreementStates.Incoming || a.State == AgreementStates.Received || a.State == AgreementStates.Opened);
            }
            else if (request.State == AgreementStates.DocInArchive)
            {
                query = employeeAgreements.Where(a => a.Document.Stage == DocStages.Archive).GroupBy(a => a.DocumentId).Select(g => g.First());
            }
            else
            {
                query = employeeAgreements.Where(a => a.State == request.State);
            }

            //List<Agreement> agreements = (request.State != AgreementStates.DocInArchive) ? query.ToList() : query.DistinctBy(a => a.DocumentId).ToList();
            List<Agreement> agreements = query.ToList();
            List<EmployeeAgreementsResponse> userAgreements = new();

            var count = 0;

            agreements.ForEach(async (a) => {
                if (a.Received == null)
                {
                    count++;
                    a.Received = DateTime.Now;
                    a.State = AgreementStates.Received;
                    await _unitOfWork.Repository<Agreement>().UpdateAsync(a);
                }

                //var employee = _userService.GetEmployeeAsync(a.UserId).Result;

                var doc = _unitOfWork.Repository<Document>().Entities.Include(d => d.Type).FirstOrDefault(d => d.Id == a.DocumentId);

                if (doc != null)
                {
                    var issuerOrg = _unitOfWork.Repository<Organization>().Entities.FirstOrDefault(o => o.Id == doc.EmplOrgId);

                    if (issuerOrg != null)
                    {
                        userAgreements.Add(new()
                        {
                            EmplOrgId = a.OrgId,
                            EmplId = a.EmplId,
                            AgreementId = a.Id,
                            ParentAgreementId = a.ParentId,

                            IssuerOrgId = doc.EmplOrgId,
                            IssuerType = issuerOrg.Type,
                            IssuerOrgInn = issuerOrg.Inn,
                            IssuerOrgShortName = issuerOrg.ShortName,

                            DocId = a.DocumentId,
                            DocParentId = doc.ParentId,
                            DocRouteId = doc.RouteId,

                            DocTypeId = doc.TypeId,
                            DocTypeName = doc.Type.Name,  // Получить
                            DocTypeShort = doc.Type.Short, // Получить

                            DocNumber = doc.Number,
                            DocDate = doc.Date,
                            DocTitle = doc.Title,

                            DocDescription = doc.Description,
                            DocURL = doc.URL,
                            DocFileName = doc.FileName,

                            DocStage = doc.Stage,
                            DocHasChanges = doc.HasChanges,
                            DocCurrentStep = doc.CurrentStep,
                            DocTotalSteps = doc.TotalSteps,

                            DocVersion = doc.Version,
                            DocCreatedBy = doc.CreatedBy,
                            DocCreatedOn = doc.CreatedOn,

                            Step = a.StageNumber,
                            State = a.State,
                            Action = a.Action,
                            IsCanceled = a.IsCanceled,

                            CreatedOn = a.CreatedOn,
                            Received = a.Received,
                            Opened = a.Opened,
                            Answered = a.Answered,

                            Remark = a.Remark,
                            SignURL = a.SignURL
                        });
                    }
                }
            });

            if (count > 0) { await _unitOfWork.Commit(cancellationToken); }

            return await Result<List<EmployeeAgreementsResponse>>.SuccessAsync(userAgreements);
        }
    }
}
