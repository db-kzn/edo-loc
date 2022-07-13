namespace EDO_FOMS.Application.Requests.Mail
{
    public class MailRequest
    {
        public string ToName { get; set; }
        public string ToAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string From { get; set; }
    }
}