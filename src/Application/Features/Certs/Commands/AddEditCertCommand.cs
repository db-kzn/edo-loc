using AutoMapper;
using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Shared.Constants.Application;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Features.Certs.Commands
{
    public class AddEditCertCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }

        [Required]
        public string Thumbprint { get; set; }
        [Required]
        public string UserId { get; set; } // User ID
        [Required]
        public string Snils { get; set; } // User SNILS

        [Required]
        public bool IsActive { get; set; }
        [Required]
        public bool SignAllowed { get; set; }
        public string IssuerInn { get; set; } // CA

        public string SerialNumber { get; set; }
        public string Algorithm { get; set; }
        public int Version { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime TillDate { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    internal class AddEditCertCommandHandler : IRequestHandler<AddEditCertCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<int> _unitOfWork;
        //private readonly IUploadService _uploadService;
        private readonly IStringLocalizer<AddEditCertCommandHandler> _localizer;

        public AddEditCertCommandHandler(
            IUnitOfWork<int> unitOfWork,
            IMapper mapper,
            // IUploadService uploadService,
            IStringLocalizer<AddEditCertCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            //_uploadService = uploadService;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(AddEditCertCommand command, CancellationToken cancellationToken)
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

                var cert = _mapper.Map<Certificate>(command);
                await _unitOfWork.Repository<Certificate>().AddAsync(cert);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, AppConstants.Cache.GetAllCertsCacheKey);
                return await Result<int>.SuccessAsync(cert.Id, _localizer["Certificate Saved"]);
            }
            else
            {
                var cert = await _unitOfWork.Repository<Certificate>().GetByIdAsync(command.Id);
                if (cert != null)
                {
                    cert.Thumbprint = command.Thumbprint;
                    cert.UserId = command.UserId;
                    cert.Snils = command.Snils;

                    cert.Snils = command.Snils;
                    cert.IsActive = command.IsActive;
                    cert.SignAllowed = command.SignAllowed;

                    await _unitOfWork.Repository<Certificate>().UpdateAsync(cert);
                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, AppConstants.Cache.GetAllCertsCacheKey);
                    return await Result<int>.SuccessAsync(cert.Id, _localizer["Certificate Updated"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["Certificate Not Found"]);
                }
            }
        }
    }
}
