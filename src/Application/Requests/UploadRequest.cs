using EDO_FOMS.Domain.Enums;

namespace EDO_FOMS.Application.Requests
{
    public class UploadRequest
    {
        public string FileName { get; set; }
        public string Extension { get; set; }

        public UploadType UploadType { get; set; }
        public byte[] Data { get; set; }
    }
}