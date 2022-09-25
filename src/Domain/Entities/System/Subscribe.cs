using EDO_FOMS.Domain.Contracts;
using Microsoft.EntityFrameworkCore;

namespace EDO_FOMS.Domain.Entities.System
{
    [Index("UserId", IsUnique = false)]
    public class Subscribe : AuditableEntity<int>
    {
        public string UserId { get; set; }
        public Notify Email { get; set; } = new();
        public Notify Telegram { get; set; } = new();
        public Notify Chat { get; set; } = new();
        public Notify Sms { get; set; } = new();

        //public Subscribe() {}
    }

    [Owned]
    public class Notify
    {
        public bool AgreementIncoming { get; set; } = true; // 1
        public bool AgreementRejected { get; set; } = false;
        public bool AgreementApproved { get; set; } = false;
        public bool AgreementSigned { get; set; } = false;
        public bool AgreementAgreed { get; set; } = false;

        public bool DocumentRejected { get; set; } = true; // 2
        public bool DocumentApproved { get; set; } = true; // 3.1
        public bool DocumentSigned { get; set; } = true;   // 3.2
        public bool DocumentAgreed { get; set; } = true;   // 4
    }
}
