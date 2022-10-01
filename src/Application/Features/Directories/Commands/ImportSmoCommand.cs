using EDO_FOMS.Application.Configurations;
using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Models.Dir;
using EDO_FOMS.Application.Responses.Directories;
using EDO_FOMS.Domain.Entities.Dir;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Shared.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace EDO_FOMS.Application.Features.Directories.Commands
{
    public class ImportSmoCommand : IRequest<Result<ImportResponse>> { }

    internal class ImportSmoCommandHandler : IRequestHandler<ImportSmoCommand, Result<ImportResponse>>
    {
        private static readonly DateTime _now = DateTime.Now;

        private readonly AppStorageInfo _storage;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<ImportSmoCommandHandler> _localizer;

        public ImportSmoCommandHandler(
            IOptions<AppStorageInfo> appStorageInfo,
            IUnitOfWork<int> unitOfWork,
            IStringLocalizer<ImportSmoCommandHandler> localizer
            )
        {
            _storage = appStorageInfo.Value;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<ImportResponse>> Handle(ImportSmoCommand request, CancellationToken cancellationToken)
        {
            var path = _storage.PathForImport;
            var F002 = Path.Combine(path, "F002.xml");

            if (!File.Exists(F002))
            {
                return await Result<ImportResponse>.FailAsync(_localizer["File does not exist"]);
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var context = new XmlParserContext(null, null, null, XmlSpace.None)
            {
                Encoding = Encoding.GetEncoding("windows-1251")
            };

            var reader = XmlReader.Create(F002, new XmlReaderSettings(), context);
            var serializer = new XmlSerializer(typeof(XmlF002));
            var xml = (XmlF002)serializer.Deserialize(reader);
            var smos = xml.SMOs;

            var response = new ImportResponse() { Total = smos.Count };

            if (response.Total > 0)
            {
                var companies = _unitOfWork.Repository<Company>();
                var orgs = _unitOfWork.Repository<Organization>();

                smos.ForEach(async smo =>
                {
                    var company = companies.Entities.FirstOrDefault(c => c.Code == smo.SmoCod);

                    if (company is null)
                    {
                        response.Added++;
                        await companies.AddAsync(NewCompany(smo));
                    }
                    else
                    {
                        if (HasChanges(company, smo))
                        {
                            response.Updated++;
                            await companies.UpdateAsync(UpdateCompany(company, smo));
                        }
                        else
                        {
                            response.Skipped++;
                        }
                    }

                    var org = orgs.Entities.FirstOrDefault(o => o.Inn == smo.Inn);
                    if (org is not null && string.IsNullOrWhiteSpace(org.OmsCode))
                    {
                        org.OmsCode = smo.SmoCod;
                        await orgs.UpdateAsync(org);
                    }
                });

                await _unitOfWork.Commit(cancellationToken);
            }

            reader.Close();
            File.Delete(F002);
            var result = _localizer["File processed and deleted"];

            return await Result<ImportResponse>.SuccessAsync(response, result);
        }

        public static Company NewCompany(SMO s)
        {
            return new Company()
            {
                Type = OrgTypes.SMO,
                State = OrgStates.Active,
                TfOkato = s.TfOkato,

                Code = s.SmoCod,
                Inn = s.Inn,
                Kpp = s.Kpp,
                Ogrn = s.Ogrn,

                Name = s.NamSmop,
                ShortName = s.NamSmok,
                Address = s.PstAddress.AddrF,
                AO = Guid.Empty,

                Phone = s.Phone,
                Fax = s.Fax,
                HotLine = s.HotLine,
                Email = s.EMail?.ToLower() ?? "",
                SiteUrl = s.Www?.ToLower() ?? "",

                HeadLastName = s.FamRuk,
                HeadName = s.ImRuk,
                HeadMidName = s.OtRuk,
                Changed = s._DEdit
            };
        }
        public static Company UpdateCompany(Company c, SMO s)
        {
            c.Type = OrgTypes.SMO;
            c.State = OrgStates.Active;
            c.TfOkato = s.TfOkato;

            c.Code = s.SmoCod;
            c.Inn = s.Inn;
            c.Kpp = s.Kpp;
            c.Ogrn = s.Ogrn;

            c.Name = s.NamSmop;
            c.ShortName = s.NamSmok;
            c.Address = s.PstAddress.AddrF;
            c.AO = Guid.Empty;

            c.Phone = s.Phone;
            c.Fax = s.Fax;
            c.HotLine = s.HotLine;
            c.Email = s.EMail?.ToLower() ?? "";
            c.SiteUrl = s.Www?.ToLower() ?? "";

            c.HeadLastName = s.FamRuk;
            c.HeadName = s.ImRuk;
            c.HeadMidName = s.OtRuk;
            c.Changed = s._DEdit;

            return c;
        }
        public static bool HasChanges(Company c, SMO s)
        {
            return c.Type != OrgTypes.SMO || c.State != OrgStates.Active || c.TfOkato != s.TfOkato ||
                c.Code != s.SmoCod || c.Inn != s.Inn || c.Kpp != s.Kpp || c.Ogrn != s.Ogrn ||
                c.Name != s.NamSmop || c.ShortName != s.NamSmok || c.Address != s.PstAddress.AddrF ||
                c.Phone != s.Phone || c.Fax != s.Fax || c.HotLine != s.HotLine ||
                c.Email != (s.EMail?.ToLower() ?? "") || c.SiteUrl != (s.Www?.ToLower() ?? "") ||
                c.HeadLastName != s.FamRuk || c.HeadName != s.ImRuk || c.HeadMidName != s.OtRuk;
        }
    }
}
