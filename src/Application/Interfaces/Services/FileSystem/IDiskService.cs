using System.Collections.Generic;

namespace EDO_FOMS.Application.Interfaces.Services.FileSystem;

public interface IDiskService
{
    int GetFilesCount(string path, string mask);
    List<string> GetImportFiles(string mask);
}
