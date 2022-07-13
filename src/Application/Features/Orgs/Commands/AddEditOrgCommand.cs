using AutoMapper;
using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Constants.Application;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Orgs.Commands
{
    public class AddEditOrgCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(10)]
        [MinLength(10)]
        public string Inn { get; set; }

        [Required]
        public string Name { get; set; }
        public string ShortName { get; set; }

        //[Required]
        //public string UserId { get; set; } // Owner - User ID
        //[Required]
        //[MaxLength(11)]
        //[MinLength(11)]
        //public string UserSnils { get; set; } // User SNISL

        public bool IsPublic { get; set; } // CA no public - hide
        public OrgTypes Type { get; set; } = OrgTypes.MO; // N/D, FOND, SMO, MO, CA
        public OrgStates State { get; set; } = OrgStates.Active; // N/D, OnSubmit, Active, Inactive, Blocked, Closed, Deleted

        public string Ogrn { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    internal class AddEditOrgCommandHandler : IRequestHandler<AddEditOrgCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<int> _unitOfWork;
        //private readonly IUploadService _uploadService;
        private readonly IStringLocalizer<AddEditOrgCommandHandler> _localizer;

        public AddEditOrgCommandHandler(
            IUnitOfWork<int> unitOfWork,
            IMapper mapper,
            // IUploadService uploadService,
            IStringLocalizer<AddEditOrgCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            //_uploadService = uploadService;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(AddEditOrgCommand command, CancellationToken cancellationToken)
        {
            //var uploadRequest = command.UploadRequest;
            //if (uploadRequest != null)
            //{
            //    uploadRequest.FileName = $"D-{Guid.NewGuid()}{uploadRequest.Extension}";
            //}

            if (command.Id == 0)
            {
                //if (uploadRequest != null)
                //{
                //    //org.URL = _uploadService.UploadAsync(uploadRequest);
                //}

                var org = _mapper.Map<Organization>(command);
                await _unitOfWork.Repository<Organization>().AddAsync(org);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, AppConstants.Cache.GetAllOrgsCacheKey);
                return await Result<int>.SuccessAsync(org.Id, _localizer["Organization Saved"]);
            }
            else
            {
                var org = await _unitOfWork.Repository<Organization>().GetByIdAsync(command.Id);
                if (org == null) { return await Result<int>.FailAsync(_localizer["Organization Not Found!"]); }

                org.Inn = command.Inn;// ?? org.Inn;
                org.Ogrn = command.Ogrn;
                org.Name = command.Name;
                org.ShortName = command.ShortName;

                //org.UserId = command.UserId;
                //org.UserSnils = command.UserSnils;

                org.IsPublic = command.IsPublic;
                org.Type = command.Type;
                org.State = command.State;

                org.Phone = command.Phone;
                org.Email = command.Email;

                //if (uploadRequest != null)
                //{
                //    //org.URL = _uploadService.UploadAsync(uploadRequest);
                //}
                //org.DocTypeId = (command.TypeId == 0) ? org.DocumentTypeId : command.DocumentTypeId;

                await _unitOfWork.Repository<Organization>().UpdateAsync(org);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, AppConstants.Cache.GetAllOrgsCacheKey);
                return await Result<int>.SuccessAsync(org.Id, _localizer["Organization Updated"]);
            }
        }
    }
}
