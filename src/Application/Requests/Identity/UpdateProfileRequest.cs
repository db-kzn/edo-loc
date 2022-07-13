using EDO_FOMS.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace EDO_FOMS.Application.Requests.Identity
{
    public class UpdateProfileRequest
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        [Required]
        public string Title { get; set; }

        [Required]
        public string Surname { get; set; }
        [Required]
        public string GivenName { get; set; }

        public string Inn { get; set; }
        public string Snils { get; set; }

        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public UserBaseRoles BaseRole { get; set; }
        public OrgTypes OrgType { get; set; }
    }
}