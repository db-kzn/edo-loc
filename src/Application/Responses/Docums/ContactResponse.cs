namespace EDO_FOMS.Application.Responses.Docums
{
    public class ContactResponse
    {
        public string Id { get; set; }             // User to Employee

        public bool IsActive { get; set; }
        public string Surname { get; set; }
        public string GivenName { get; set; }

        public int OrgId { get; set; }
        public string InnLe { get; set; }
        public string OrgShortName { get; set; }
    }
}
