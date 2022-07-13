using EDO_FOMS.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace EDO_FOMS.Domain.Entities.Org
{
    public class Employee
    {
        public string UserId { get; set; }
        public int EmployeeId { get; set; }

        public int OrgId { get; set; }

        [MaxLength(10)]
        public string InnLe { get; set; }
        [MaxLength(11)]
        public string Snils { get; set; }
        [MaxLength(12)]
        public string Inn { get; set; }

        [MaxLength(200)]
        public string Title { get; set; }
        [MaxLength(200)]
        public string Surname { get; set; }
        [MaxLength(200)]
        public string GivenName { get; set; }

        public OrgTypes OrgType { get; set; } = OrgTypes.Undefined;  // N/D, FOND, SMO, MO, CA
        public UserBaseRoles BaseRole { get; set; } = UserBaseRoles.Employee; // N/D, Sys Admin, Chief, Employee
        public bool IsActive { get; set; } = true;
    }
}
