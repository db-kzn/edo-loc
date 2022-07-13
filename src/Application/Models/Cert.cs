namespace EDO_FOMS.Application.Models
{
    public class Cert
    {
        public bool HasPrivateKey { get; set; }
        public bool IsValid { get; set; }
        public bool IsCorrect { get; set; }
        public bool IsOrgCert { get; set; }

        //public string Name { get; set; }
        //public string EMail { get; set; }

        public string Algorithm { get; set; }
        public string Thumbprint { get; set; }
        public string SerialNumber { get; set; }

        public string FromDate { get; set; }
        public string TillDate { get; set; }
        public int Version { get; set; }

        public string Provider { get; set; }
        public string PrivateKeyLink { get; set; }

        public CertSubject Subject { get; set; }
        //public CertOrg Org { get; set; }
        public CertIssuer Issuer { get; set; }
    }

    public class CertSubject
    {
        // USER
        public string Surname { get; set; }
        public string GivenName { get; set; }
        public string Title { get; set; } = "";
        public string Inn { get; set; }
        public string Snils { get; set; }
        // ORG
        public string Org { get; set; } = "";
        public string Ogrn { get; set; }
        public string InnLe { get; set; }
        // CERT
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class CertIssuer
    {
        public string Name { get; set; }
        public string Inn { get; set; }
        public string Ogrn { get; set; }
        public string EMail { get; set; }
    }
}
