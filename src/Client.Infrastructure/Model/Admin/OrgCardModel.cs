using EDO_FOMS.Domain.Enums;
using System;

namespace EDO_FOMS.Client.Infrastructure.Model.Admin
{
    public class OrgCardModel
    {
        public int Id { get; set; } = 0;
        public string Inn { get; set; } = string.Empty;
        public string Ogrn { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;

        public string HeadId { get; set; } = string.Empty; // Head - User ID
        public string BuhgId { get; set; } = string.Empty; // Buhg - User ID

        public bool IsPublic { get; set; } = true; // CA no public - hide
        public OrgTypes Type { get; set; } = OrgTypes.MO; // N/D, FOND, SMO, MO, CA
        public OrgStates State { get; set; } = OrgStates.Active; // N/D, OnSubmit, Active, Inactive, Blocked, Closed, Deleted

        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? CreatedOn { get; set; } = null;
    }
}
