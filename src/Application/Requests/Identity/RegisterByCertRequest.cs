using EDO_FOMS.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace EDO_FOMS.Application.Requests.Identity
{
    public class RegisterByCertRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }
        [Required]
        [MaxLength(500)]
        public string Org { get; set; }

        [Required]
        [MaxLength(10)]
        //[MinLength(10)]
        public string InnLe { get; set; }
        [Required]
        [MaxLength(11)]
        //[MinLength(11)]
        public string Snils { get; set; }

        [Required]
        public string Surname { get; set; }
        [Required]
        public string GivenName { get; set; }

        [Required]
        public string Thumbprint { get; set; }
        [Required]
        public DateTime FromDate { get; set; } = DateTime.Now;
        [Required]
        public DateTime TillDate { get; set; } = DateTime.Now;

        public string Inn { get; set; }
        public OrgTypes OrgType { get; set; } = OrgTypes.MO;
        public UserBaseRoles BaseRole { get; set; } = UserBaseRoles.User;

        public string Ogrn { get; set; }
        public string Name { get; set; } // Cert Owner

        public string Email { get; set; }
        //public string PhoneNumber { get; set; }

        //public string Role { get; set; } = RoleConstants.BasicRole;
    }
}
