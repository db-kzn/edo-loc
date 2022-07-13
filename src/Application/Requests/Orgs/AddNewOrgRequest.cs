using System.ComponentModel.DataAnnotations;

namespace EDO_FOMS.Application.Requests.Orgs
{
    public class AddNewOrgRequest
    {
        [Required]
        public string OrgName { get; set; }

        public string OrgOwner { get; set; }
        [Required]
        [MaxLength(12)]
        public string Inn { get; set; }

        [MaxLength(13)]
        public string Ogrn { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }


        public string CommonName { get; set; }

        [MaxLength(128)]
        public string StreetAdress { get; set; }
        [MaxLength(128)]
        public string Locality { get; set; }
        [MaxLength(128)]
        public string State { get; set; }
        [MaxLength(2)]
        public string Country { get; set; }

        public bool IsOwner { get; set; } = true;
        public bool ActivateOrg { get; set; } = false;
    }
}
