using EDO_FOMS.Domain.Enums;
using System;

namespace EDO_FOMS.Client.Infrastructure.Model.Admin
{
    public class UserCardModel
    {
        public string Id { get; set; }
        public string InnLe { get; set; }
        public string Snils { get; set; }

        public string Inn { get; set; }
        public string Title { get; set; }

        public string Surname { get; set; }
        public string GivenName { get; set; }

        public UserBaseRoles BaseRole { get; set; } = UserBaseRoles.Employee;
        public OrgTypes OrgType { get; set; } = OrgTypes.MO;

        public int OrgId { get; set; }
        public string OrgName { get; set; }
        public string OrgShort { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public bool IsActive { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }

        public string ProfilePictureDataUrl { get; set; } = "";
        public DateTime CreatedOn { get; set; }
    }
}
