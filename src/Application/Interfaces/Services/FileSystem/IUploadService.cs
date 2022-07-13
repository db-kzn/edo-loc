using EDO_FOMS.Application.Models;
using EDO_FOMS.Application.Requests;

namespace EDO_FOMS.Application.Interfaces.Services.FileSystem;

public interface IUploadService
{
    string Upload(UploadRequest request);
    UploadResult UploadDoc(UploadRequest request, int ver, string path);
    UploadResult ArchiveDoc(string path, string name);
    bool UploadSign(byte[] Data, string path, string name);
    bool DeleteFolder(string path);
}