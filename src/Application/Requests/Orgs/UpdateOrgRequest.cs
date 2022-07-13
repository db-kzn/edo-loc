using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Requests.Orgs
{
    public class UpdateOrgRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(12)]
        public string Inn { get; set; }


        [Required]
        public string Name { get; set; }
        public string UserId { get; set; }
        public string UserSnils { get; set; }


        [MaxLength(13)]
        public string Ogrn { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Phone { get; set; }

        //[MaxLength(2)]
        //public string Country { get; set; }
        //[MaxLength(128)]
        //public string Locality { get; set; }
        //[MaxLength(128)]
        //public string State { get; set; }
        //[MaxLength(128)]
        //public string StreetAdress { get; set; }
    }
}
