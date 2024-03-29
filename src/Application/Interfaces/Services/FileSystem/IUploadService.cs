﻿using EDO_FOMS.Application.Interfaces.Common;
using EDO_FOMS.Application.Models;
using EDO_FOMS.Application.Requests;

namespace EDO_FOMS.Application.Interfaces.Services.FileSystem;

public interface IUploadService : IService
{
    string Upload(UploadRequest request);
    UploadResult UploadDoc(UploadRequest request, int ver, string path);
    UploadResult ImportDoc(string importFileName);
    UploadResult ArchiveDoc(string path, string name);
    bool UploadSign(byte[] Data, string path, string name);
    bool DeleteFolder(string path);
}