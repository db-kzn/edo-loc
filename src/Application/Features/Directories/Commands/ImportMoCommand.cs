using EDO_FOMS.Application.Configurations;
using EDO_FOMS.Application.Interfaces.Repositories;
using EDO_FOMS.Application.Models.Dir;
using EDO_FOMS.Domain.Entities.Dir;
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
    public class ImportMoCommand : IRequest<Result<ImportResponse>> { }

    internal class ImportMoCommandHandler : IRequestHandler<ImportMoCommand, Result<ImportResponse>>
    {
        private static readonly DateTime _now = DateTime.Now;

        private readonly AppStorageInfo _storage;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<ImportMoCommandHandler> _localizer;

        public ImportMoCommandHandler(
            IOptions<AppStorageInfo> appStorageInfo,
            IUnitOfWork<int> unitOfWork,
            IStringLocalizer<ImportMoCommandHandler> localizer
            )
        {
            _storage = appStorageInfo.Value;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<ImportResponse>> Handle(ImportMoCommand request, CancellationToken cancellationToken)
        {
            var path = _storage.PathForImport;
            var F003 = Path.Combine(path, "F003.xml");

            if (!File.Exists(F003))
            {
                return await Result<ImportResponse>.FailAsync(_localizer["File does not exist"]);
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var context = new XmlParserContext(null, null, null, XmlSpace.None)
            {
                Encoding = Encoding.GetEncoding("windows-1251")
            };

            var reader = XmlReader.Create(F003, new XmlReaderSettings(), context);
            var serializer = new XmlSerializer(typeof(XmlF003));
            var xml = (XmlF003)serializer.Deserialize(reader);
            var mos = xml.MOs;

            var response = new ImportResponse() { Total = mos.Count };

            if (response.Total > 0)
            {
                var companies = _unitOfWork.Repository<Company>();

                mos.ForEach(async mo =>
                {
                    Company company = companies.Entities.FirstOrDefault(c => c.Code == mo.MCod);

                    if (company is null)
                    {
                        response.Added++;
                        await companies.AddAsync(NewCompany(mo));
                    }
                    else
                    {
                        if (HasChanges(company, mo))
                        {
                            response.Updated++;
                            await companies.UpdateAsync(UpdateCompany(company, mo));
                        }
                        else
                        {
                            response.Skipped++;
                        }
                    }
                });

                await _unitOfWork.Commit(cancellationToken);
            }

            reader.Close();
            File.Delete(F003);
            var result = _localizer["File processed and deleted"];

            return await Result<ImportResponse>.SuccessAsync(response, result);
        }

        public static Company NewCompany(MO m)
        {
            return new Company()
            {
                Type = OrgTypes.MO,
                State = OrgStates.Active,
                TfOkato = m.TfOkato,

                Code = m.MCod,
                Inn = m.Inn,
                Kpp = m.Kpp,
                Ogrn = m.Ogrn,

                Name = m.NamMop,
                ShortName = m.NamMok,
                Address = m.JurAddress.AddrJ,
                AO = Guid.Empty,

                Phone = m.Phone,
                Fax = m.Fax,
                HotLine = "",
                Email = m.EMail?.ToLower() ?? "",
                SiteUrl = m.Www?.ToLower() ?? "",

                HeadLastName = m.FamRuk,
                HeadName = m.ImRuk,
                HeadMidName = m.OtRuk,
                Changed = m._DEdit
            };
        }
        public static Company UpdateCompany(Company c, MO m)
        {
            c.Type = OrgTypes.MO;
            c.State = OrgStates.Active;
            c.TfOkato = m.TfOkato;

            c.Code = m.MCod;
            c.Inn = m.Inn;
            c.Kpp = m.Kpp;
            c.Ogrn = m.Ogrn;

            c.Name = m.NamMop;
            c.ShortName = m.NamMok;
            c.Address = m.JurAddress.AddrJ;
            c.AO = Guid.Empty;

            c.Phone = m.Phone;
            c.Fax = m.Fax;
            c.HotLine = "";
            c.Email = m.EMail?.ToLower() ?? "";
            c.SiteUrl = m.Www?.ToLower() ?? "";

            c.HeadLastName = m.FamRuk;
            c.HeadName = m.ImRuk;
            c.HeadMidName = m.OtRuk;
            c.Changed = m._DEdit;

            return c;
        }
        public static bool HasChanges(Company c, MO m)
        {
            return c.Type != OrgTypes.MO || c.State != OrgStates.Active || c.TfOkato != m.TfOkato ||
                c.Code != m.MCod || c.Inn != m.Inn || c.Kpp != m.Kpp || c.Ogrn != m.Ogrn ||
                c.Name != m.NamMop || c.ShortName != m.NamMok || c.Address != m.JurAddress.AddrJ ||
                c.Phone != m.Phone || c.Fax != m.Fax ||
                c.Email != (m.EMail?.ToLower() ?? "") || c.SiteUrl != (m.Www?.ToLower() ?? "") ||
                c.HeadLastName != m.FamRuk || c.HeadName != m.ImRuk || c.HeadMidName != m.OtRuk;
        }
    }
}
