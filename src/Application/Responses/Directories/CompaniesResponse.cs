using EDO_FOMS.Domain.Enums;
using System;

namespace EDO_FOMS.Application.Responses.Directories
{
    public class CompaniesResponse
    {
        public int Id { get; set; }
        public OrgTypes Type { get; set; } = OrgTypes.MO; // N/D, FOND, SMO, MO, CA
        public OrgStates State { get; set; } = OrgStates.Active;// N/D, OnSubmit, Active, Inactive, Blocked, Closed, Deleted
        public int? Region { get; set; } = null;
        public string TfOkato { get; set; } = "";

        public string Code { get; set; } = "";
        public string Inn { get; set; } = "";
        public string Kpp { get; set; } = "";
        public string Ogrn { get; set; } = "";

        public string Name { get; set; } = "";
        public string ShortName { get; set; } = "";
        public string Address { get; set; } = "";
        public Guid? AO { get; set; } = null;

        public string Phone { get; set; } = "";
        public string Fax { get; set; } = "";
        public string HotLine { get; set; } = "";
        public string Email { get; set; } = "";
        public string SiteUrl { get; set; } = "";

        public string HeadName { get; set; } = "";
        public string HeadLastName { get; set; } = "";
        public string HeadMidName { get; set; } = "";
        public DateTime? Changed { get; set; } = null;
    }
}
