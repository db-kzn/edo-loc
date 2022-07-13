using System;

namespace EDO_FOMS.Application.Features.Certs.Queries
{
    public class CertsResponse
    {
        public int Id { get; set; }

        public string Thumbprint { get; set; }
        public string UserId { get; set; } // User ID
        public string Snils { get; set; } // User SNILS

        public bool IsActive { get; set; }
        public bool SignAllowed { get; set; }
        public string IssuerInn { get; set; } // CA

        public string SerialNumber { get; set; }
        public string Algorithm { get; set; }
        public int Version { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime TillDate { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
