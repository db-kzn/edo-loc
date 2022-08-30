namespace EDO_FOMS.Client.Models
{
    public class FileParseModel
    {
        public string FileName { get; set; } = string.Empty;
        public string FileMask { get; set; } = string.Empty;
        public string FileAccept { get; set; } = string.Empty;

        public string DocTitle { get; set; } = string.Empty;
        public string DocNumber { get; set; } = string.Empty;
        public string DocDate { get; set; } = string.Empty;
        public string DocNotes { get; set; } = string.Empty;

        public string CodeMo { get; set; } = string.Empty;
        public string CodeSmo { get; set; } = string.Empty;
        public string CodeFund { get; set; } = string.Empty;
    }
}
