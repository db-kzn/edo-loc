﻿using AutoMapper;
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
        [Required, MaxLength(12), MinLength(10)]
        public string Inn { get; set; }
        [MaxLength(13)]
        public string Ogrn { get; set; } = string.Empty;

        [MaxLength(6)]
        public string OmsCode { get; set; }
        [Required]
        [MaxLength(500)]
        public string Name { get; set; }
        [MaxLength(32)]
        public string ShortName { get; set; }

        public string HeadId { get; set; } = string.Empty; // Head - User ID
        public string BuhgId { get; set; } = string.Empty; // Buhg - User ID

        public bool IsPublic { get; set; } // CA no public - hide
        public OrgTypes Type { get; set; } = OrgTypes.MO; // N/D, FOND, SMO, MO, CA
        public OrgStates State { get; set; } = OrgStates.Active; // N/D, OnSubmit, Active, Inactive, Blocked, Closed, Deleted

        public string Phone { get; set; }
        public string Email { get; set; }

        //[Required]
        //public string UserId { get; set; } // Owner - User ID
        //[Required]
        //[MaxLength(11)]
        //[MinLength(11)]
        //public string UserSnils { get; set; } // User SNISL
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
                org.OmsCode = command.OmsCode;
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
                _ = await _unitOfWork.CommitAndRemoveCache(cancellationToken, AppConstants.Cache.GetAllOrgsCacheKey);

                return await Result<int>.SuccessAsync(org.Id, _localizer["Organization Updated"]);
            }
        }
    }
}
