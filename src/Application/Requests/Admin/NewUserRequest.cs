using EDO_FOMS.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace EDO_FOMS.Application.Requests.Admin
{
    public class NewUserRequest
    {
        [Required]
        [MaxLength(10)]
        public string InnLe { get; set; }
        [Required]
        [MaxLength(11)]
        public string Snils { get; set; }
        public string Inn { get; set; }

        public string Title { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string GivenName { get; set; }

        public OrgTypes OrgType { get; set; } = OrgTypes.MO;
        public UserBaseRoles BaseRole { get; set; } = UserBaseRoles.Employee;

        public string Ogrn { get; set; }
        public string OrgName { get; set; }
        public string Email { get; set; }
        //public string PhoneNumber { get; set; }

        [Required]
        public string Thumbprint { get; set; }
        [Required]
        public DateTime? FromDate { get; set; } = DateTime.Now;
        [Required]
        public DateTime? TillDate { get; set; } = DateTime.Now;

        //public string Role { get; set; } = RoleConstants.BasicRole;
        public bool IsActive { get; set; } = true;
        public bool EmailConfirmed { get; set; } = true;
    }
}
