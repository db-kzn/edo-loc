using System;
using System.ComponentModel.DataAnnotations;

namespace EDO_FOMS.Application.Requests.Identity
{
    public class CertCheckRequest
    {
        public string OrgInn { get; set; }
        public string OrgName { get; set; }

        public string Surname { get; set; }
        public string GivenName { get; set; }

        [Required]
        public string Snils { get; set; }
        [Required]
        public string Thumbprint { get; set; }

        [Required]
        public DateTime FromDate { get; set; } = DateTime.Now;
        [Required]
        public DateTime TillDate { get; set; } = DateTime.Now;
    }
}
