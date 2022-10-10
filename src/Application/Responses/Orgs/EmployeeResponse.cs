using EDO_FOMS.Domain.Enums;
using System;

namespace EDO_FOMS.Application.Responses.Orgs
{
    public class EmployeeResponse
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