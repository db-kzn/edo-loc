using EDO_FOMS.Domain.Contracts;
using System.ComponentModel.DataAnnotations;

namespace EDO_FOMS.Domain.Entities.Org
{
    public class Employee : AuditableEntity<int>
    {
        public int OrgId { get; set; }
        public Organization Organization { get; set; }
        public int? DepartmentId { get; set; } = null;
        public Department Department { get; set; }
        public int? JobTitleId { get; set; } = null;
        public JobTitle JobTitle { get; set; }

        public string UserId { get; set; } = null;            // Пользователь системы
        public string ChangerId { get; set; } = null;         // Пользователь - Сменщик

        [MaxLength(10)]
        public string InnLe { get; set; } = string.Empty;
        [MaxLength(11)]
        public string Snils { get; set; } = string.Empty;
        [MaxLength(12)]
        public string Inn { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;     // Обращение: Фамилия ИО
        [MaxLength(200)]
        public string Surname { get; set; } = string.Empty;   // Фамилия
        [MaxLength(200)]
        public string GivenName { get; set; } = string.Empty; // Имя (Отчество)
    }
}
