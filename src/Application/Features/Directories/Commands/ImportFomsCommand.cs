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
    public class ImportFomsCommand : IRequest<Result<ImportResponse>> { }

    internal class ImportFomsCommandHandler : IRequestHandler<ImportFomsCommand, Result<ImportResponse>>
    {
        private static readonly DateTime _now = DateTime.Now;

        private readonly AppStorageInfo _storage;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<ImportFomsCommandHandler> _localizer;

        public ImportFomsCommandHandler(
            IOptions<AppStorageInfo> appStorageInfo,
            IUnitOfWork<int> unitOfWork,
            IStringLocalizer<ImportFomsCommandHandler> localizer
            )
        {
            _storage = appStorageInfo.Value;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<ImportResponse>> Handle(ImportFomsCommand request, CancellationToken cancellationToken)
        {
            var path = _storage.PathForImport;
            var F001 = Path.Combine(path, "F001.xml");

            if (!File.Exists(F001))
            {
                return await Result<ImportResponse>.FailAsync(_localizer["File does not exist"]); 
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var context = new XmlParserContext(null, null, null, XmlSpace.None)
            {
                Encoding = Encoding.GetEncoding("windows-1251")
            };

            var reader = XmlReader.Create(F001, new XmlReaderSettings() { }, context);
            var serializer = new XmlSerializer(typeof(XmlF001));
            var xml = (XmlF001)serializer.Deserialize(reader);
            var tfomss = xml.TFOMSs;

            var response = new ImportResponse() { Total = tfomss.Count };

            if (response.Total > 0)
            {
                var companies = _unitOfWork.Repository<Company>();
                var orgs = _unitOfWork.Repository<Organization>();

                tfomss.ForEach(async tfoms =>
                {
                    var company = companies.Entities.FirstOrDefault(c => c.Code == tfoms.TfKod);

                    if (company is null)
                    {
                        response.Added++;
                        await companies.AddAsync(NewCompany(tfoms));
                    }
                    else
                    {
                        if (HasChanges(company, tfoms))
                        {
                            response.Updated++;
                            await companies.UpdateAsync(UpdateCompany(company, tfoms));
                        }
                        else
                        {
                            response.Skipped++;
                        }
                    }

                    var org = orgs.Entities.FirstOrDefault(o => o.Inn == tfoms.Mtr.Inn);
                    if (org is not null && string.IsNullOrWhiteSpace(org.OmsCode))
                    {
                        org.OmsCode = tfoms.TfKod;
                        await orgs.UpdateAsync(org);
                    }
                });

                await _unitOfWork.Commit(cancellationToken);
            }

            reader.Close();
            File.Delete(F001);
            var result = _localizer["File processed and deleted"];

            return await Result<ImportResponse>.SuccessAsync(response, result);
        }

        public static Company NewCompany(TFOMS f)
        {
            return new Company()
            {
                Type = OrgTypes.Fund,
                State = OrgStates.Active,
                TfOkato = f.TfOkato,

                Code = f.TfKod,
                Inn = f.Mtr.Inn,
                Kpp = f.Mtr.Kpp,
                Ogrn = f.TfOgrn,

                Name = f.NameTfp,
                ShortName = f.NameTfk,
                Address = f.Address,
                AO = Guid.Empty,

                Phone = f.Phone,
                Fax = f.Fax,
                HotLine = f.HotLine,
                Email = f.EMail?.ToLower() ?? "",
                SiteUrl = f.Www?.ToLower() ?? "",

                HeadLastName = f.FamDir,
                HeadName = f.ImDir,
                HeadMidName = f.OtDir,
                Changed = f._DEdit
            };
        }
        public static Company UpdateCompany(Company c, TFOMS f)
        {
            c.Type = OrgTypes.Fund;
            c.State = OrgStates.Active;
            c.TfOkato = f.TfOkato;

            c.Code = f.TfKod;
            c.Inn = f.Mtr.Inn;
            c.Kpp = f.Mtr.Kpp;
            c.Ogrn = f.TfOgrn;

            c.Name = f.NameTfp;
            c.ShortName = f.NameTfk;
            c.Address = f.Address;
            c.AO = Guid.Empty;

            c.Phone = f.Phone;
            c.Fax = f.Fax;
            c.HotLine = f.HotLine;
            c.Email = f.EMail?.ToLower() ?? "";
            c.SiteUrl = f.Www?.ToLower() ?? "";

            c.HeadLastName = f.FamDir;
            c.HeadName = f.ImDir;
            c.HeadMidName = f.OtDir;
            c.Changed = f._DEdit;

            return c;
        }
        public static bool HasChanges(Company c, TFOMS f)
        {
            return c.Type != OrgTypes.Fund ||
                c.State != OrgStates.Active ||
                c.TfOkato != f.TfOkato ||

                c.Code != f.TfKod ||
                c.Inn != f.Mtr.Inn ||
                c.Kpp != f.Mtr.Kpp ||
                c.Ogrn != f.TfOgrn ||

                c.Name != f.NameTfp ||
                c.ShortName != f.NameTfk ||
                c.Address != f.Address ||

                c.Phone != f.Phone ||
                c.Fax != f.Fax ||
                c.HotLine != f.HotLine ||
                c.Email != (f.EMail?.ToLower() ?? "") ||
                c.SiteUrl != (f.Www?.ToLower() ?? "") ||

                c.HeadLastName != f.FamDir ||
                c.HeadName != f.ImDir ||
                c.HeadMidName != f.OtDir;
        }
    }
}
