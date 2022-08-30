using EDO_FOMS.Domain.Contracts;
using EDO_FOMS.Domain.Entities.Dir;

namespace EDO_FOMS.Domain.Entities.Doc
{
    public class DocPacketFile : AuditableEntity<int>
    {
        // Id
        public int DocumentId { get; set; }
        public Document Document { get; set; }

        public int? RoutePacketFileId { get; set; }
        public RoutePacketFile RoutePacketFile { get; set; }

        public string URL { get; set; }
        public string StoragePath { get; set; }
        public string Name { get; set; }
    }
}
