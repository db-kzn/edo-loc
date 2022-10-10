using EDO_FOMS.Domain.Enums;
using System;
using System.Collections.Generic;

namespace EDO_FOMS.Client.Infrastructure.Model.Admin
{
    public class OrgCardModel
    {
        public List<OrgCardUserModel> Users { get; set; } = new();

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

    public class OrgCardUserModel
    {
        public string Id { get; set; }
        public string InnLe { get; set; }
        public string Snils { get; set; }
        public string Inn { get; set; }

        public string Title { get; set; }
        public string UserName { get; set; }
        public string Surname { get; set; }
        public string GivenName { get; set; }

        public OrgTypes OrgType { get; set; }
        public UserBaseRoles BaseRole { get; set; }
        public bool IsActive { get; set; }

        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }

        public string ProfilePictureDataUrl { get; set; } = "";
        public DateTime CreatedOn { get; set; }
    }
}
