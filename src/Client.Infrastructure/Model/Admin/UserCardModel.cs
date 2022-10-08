using EDO_FOMS.Domain.Enums;

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

        public OrgTypes OrgType { get; set; } = OrgTypes.MO;
        public UserBaseRoles BaseRole { get; set; } = UserBaseRoles.Employee;

        public bool IsActive { get; set; } = true;
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; } = true;
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; } = true;
    }
}
