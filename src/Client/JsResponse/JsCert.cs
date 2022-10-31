using System;

namespace EDO_FOMS.Client.JsResponse
{
    public class JsCert
    {
        public int Ix { get; set; }
        public bool FromContainer { get; set; }
        public bool HasError { get; set; }

        public string Thumbprint { get; set; }
        public string SerialNumber { get; set; }
        public int Type { get; set; }  // enum CertTypes

        public DateTime? FromDate { get; set; }
        public DateTime? TillDate { get; set; }
        public int Version { get; set; }

        public string Provider { get; set; }
        public string PrivateKeyLink { get; set; }
        public bool HasPrivateKey { get; set; }

        public bool IsCorrect { get; set; }
        public bool IsValid { get; set; }

        public JsPublicKey PublicKey { get; set; }
        public JsCertSubject Subject { get; set; }
        public JsCertIssuer Issuer { get; set; }
    }
    public class JsPublicKey
    {
        public string Algorithm { get; set; }
        public string Oid { get; set; }
        public string Key { get; set; }
        public string Parameters { get; set; }
        public int Length { get; set; }
    }
    public class JsCertSubject
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

        public string Street { get; set; }
        public string Locality { get; set; }
        public string Statet { get; set; }
        public string Country { get; set; }
    }
    public class JsCertIssuer
    {
        public string Name { get; set; }
        public string Inn { get; set; }
        public string Ogrn { get; set; }
        public string EMail { get; set; }
    }
}
