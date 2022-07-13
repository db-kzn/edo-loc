using EDO_FOMS.Domain.Contracts;
using System;
using System.ComponentModel.DataAnnotations;

namespace EDO_FOMS.Domain.Entities.Org
{
    public class Certificate : AuditableEntity<int>
    {
        [Required]
        [MaxLength(40)]
        public string Thumbprint { get; set; } = "";
        [Required]
        [MaxLength(36)]
        public string UserId { get; set; } // User ID
        [Required]
        [MaxLength(11)]
        public string Snils { get; set; } // User SNILS

        [Required]
        public bool IsActive { get; set; } = true;// Sys Admin
        [Required]
        public bool SignAllowed { get; set; } = false; // Org Chief
        [MaxLength(10)]
        public string IssuerInn { get; set; } // CA

        public string SerialNumber { get; set; }
        public string Algorithm { get; set; }
        public int Version { get; set; } = 3;

        public string OpenKey { get; set; } // Открытый ключ
        public DateTime FromDate { get; set; } = DateTime.Now;
        public DateTime TillDate { get; set; } = DateTime.Now;
    }
}
