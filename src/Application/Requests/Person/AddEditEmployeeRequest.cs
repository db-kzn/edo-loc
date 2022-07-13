using EDO_FOMS.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace EDO_FOMS.Application.Requests.Person
{
    public class AddEditEmployeeRequest
    {
        //[Required]
        public string Id { get; set; } = "";
        public string Title { get; set; }

        [Required]
        public string Surname { get; set; }
        [Required]
        public string GivenName { get; set; }

        [Required]
        [MaxLength(11)]
        public string Snils { get; set; }
        public string Inn { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public UserBaseRoles BaseRole { get; set; } = UserBaseRoles.Employee;
        public bool IsActive { get; set; } = false;

        //[Required]
        //[MaxLength(10)]
        //public string InnLe { get; set; }
        //public int OrgTypeIx { get; set; } = (int)OrgTypes.MO;
        //public bool EmailConfirmed { get; set; } = false;
        //public bool PhoneConfirmed { get; set; } = false;
    }
}
