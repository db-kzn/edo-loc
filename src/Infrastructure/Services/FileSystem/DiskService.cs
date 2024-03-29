﻿using EDO_FOMS.Application.Configurations;
using EDO_FOMS.Application.Interfaces.Services.FileSystem;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EDO_FOMS.Infrastructure.Services.FileSystem;

public class DiskService : IDiskService
{
    private readonly AppStorageInfo _storage;
    private readonly ILogger<DiskService> _logger;

    public DiskService(
        IOptions<AppStorageInfo> appStorageInfo,
        ILogger<DiskService> logger
        )
    {
        _storage = appStorageInfo.Value;
        _logger = logger;
    }

    public int GetFilesCount(string type, string mask)
    {
        if (string.IsNullOrEmpty(type)) { return 0; }
        //_logger.LogInformation("Path for type {path} is {_path}", type, _storage.PathForImport);

        var path = (type == "ForDocsImport") ? _storage.PathForImport : "";
        //var mask = string.IsNullOrWhiteSpace(_storage.ImportFileMask) ? "*.xls" : _storage.ImportFileMask;

        if (!Directory.Exists(path)) { return 0; }

        var dir = new DirectoryInfo(path);
        var count = dir.GetFiles(mask ?? string.Empty).Length;

        _logger.LogInformation("Path Exists. Contains files: {count}", count);

        return count;
    }

    public List<string> GetImportFiles(string mask)
    {
        var path = _storage.PathForImport;
        if (!Directory.Exists(path)) { return new(); }

        var dir = new DirectoryInfo(path);
        var files = dir.GetFiles(mask ?? string.Empty).Select(fileInfo => fileInfo.Name).ToList();

        return files;
    }
}
