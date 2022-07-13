using EDO_FOMS.Application.Extensions;
using EDO_FOMS.Application.Interfaces.Services.FileSystem;
using EDO_FOMS.Application.Models;
using EDO_FOMS.Application.Requests;
using EDO_FOMS.Domain.Enums;
using System;
using System.IO;
using System.IO.Compression;

namespace EDO_FOMS.Infrastructure.Services.FileSystem
{
    public class UploadService : IUploadService
    {
        private const string numberPattern = "_({0})";

        public string Upload(UploadRequest request)
        {
            if (request.Data == null) return string.Empty;
            var streamData = new MemoryStream(request.Data);

            if (streamData.Length > 0)
            {
                var currentDir = Directory.GetCurrentDirectory();
                var dirRoot = Directory.GetDirectoryRoot(currentDir);

                var folderByType = Path.Combine("Files", request.UploadType.ToDescriptionString());
                var storagePath = Path.Combine("home", "edo", folderByType);

                var pathToSave = Path.Combine(dirRoot, storagePath);

                bool exists = Directory.Exists(pathToSave);
                if (!exists) { Directory.CreateDirectory(pathToSave); }

                var fileName = request.FileName.Trim('"');

                var fullPath = Path.Combine(pathToSave, fileName);
                var dbPath = Path.Combine(storagePath, fileName);

                if (File.Exists(dbPath))
                {
                    dbPath = NextAvailableFilename(dbPath);
                    fullPath = NextAvailableFilename(fullPath);
                }

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    streamData.CopyTo(stream);
                }

                return dbPath;
            }
            else
            {
                return string.Empty;
            }
        }

        public UploadResult UploadDoc(UploadRequest request, int ver = 1, string prevStoragePath = "")
        {
            if (request.Data == null) { return null; } // string.Empty;
            var streamData = new MemoryStream(request.Data);
            if (streamData.Length == 0) { return null; } // string.Empty;

            var currentDir = Directory.GetCurrentDirectory();
            var dirRoot = Directory.GetDirectoryRoot(currentDir);
            var basePath = Path.Combine(dirRoot, "home", "edo"); // Base Path on the Storage

            if (!string.IsNullOrWhiteSpace(prevStoragePath))
            {
                // Delete Folder by prevStoragePath
                var deletePath = Path.Combine(basePath, prevStoragePath);
                Directory.Delete(deletePath, true);
            }

            var storagePath = Path.Combine("Files", request.UploadType.ToDescriptionString()); // Files\docs

            var fileName = request.FileName.Trim('"'); // Path.GetFileName(request.FileName)

            if (request.UploadType == UploadType.Document)
            {
                var nowDate = DateTime.Today;
                var year = nowDate.ToString("yyyy");
                var month = nowDate.ToString("MM");
                var day = nowDate.ToString("dd");

                var fileBase = Path.GetFileNameWithoutExtension(fileName);
                if (ver > 1) { fileBase += $"_v{ver}"; }

                var docPath = Path.Combine(year, month, day, fileBase);
                storagePath = Path.Combine(storagePath, docPath);
            }

            var pathToSave = Path.Combine(basePath, storagePath);

            if (Directory.Exists(pathToSave))
            {
                storagePath = GetNextUrlName(basePath, storagePath);
                pathToSave = Path.Combine(basePath, storagePath);
            }

            Directory.CreateDirectory(pathToSave);

            var fullUrl = Path.Combine(storagePath, fileName);
            var fullPath = Path.Combine(basePath, fullUrl);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                streamData.CopyTo(stream);
            }

            UploadResult result = new()
            {
                URL = fullUrl,
                StoragePath = storagePath,
                FileName = fileName
            };

            return result;
        }

        public UploadResult ArchiveDoc(string storagePath, string fileName)
        {
            var currentDir = Directory.GetCurrentDirectory();
            var dirRoot = Directory.GetDirectoryRoot(currentDir);
            var basePath = Path.Combine(dirRoot, "home", "edo"); // Base Path on the Storage

            var docPath = Path.Combine(basePath, storagePath);

            if (!Directory.Exists(docPath)) { return null; } // Нечего архивировать

            var fileTitle = Path.GetFileNameWithoutExtension(fileName);
            var archName = fileTitle + ".zip";
            var archivePath = Path.Combine("Files", "arcs");

            var nowDate = DateTime.Today;
            var year = nowDate.ToString("yyyy");
            var month = nowDate.ToString("MM");
            var day = nowDate.ToString("dd");
            var pathByDay = Path.Combine(year, month, day);

            archivePath = Path.Combine(archivePath, pathByDay);
            var pathToArch = Path.Combine(basePath, archivePath);
            if (!Directory.Exists(pathToArch)) { Directory.CreateDirectory(pathToArch); }

            var arcUrl = Path.Combine(archivePath, archName);
            var fullPath = Path.Combine(basePath, arcUrl);
            if (File.Exists(fullPath))
            {
                arcUrl = GetNextArcPath(basePath, archivePath, fileTitle);
                fullPath = Path.Combine(basePath, arcUrl);
            }
            ZipFile.CreateFromDirectory(docPath, fullPath);

            var result = new UploadResult()
            {
                URL = arcUrl,              // Полный путь для загрузки архива
                StoragePath = archivePath, // Путь сохранения архива
                FileName = archName        // Исходное имя с заменой PDF на ZIP
            };

            return result;
        }

        public bool UploadSign(byte[] Data, string path, string name)
        {
            var signName = CheckName(name) + ".sig";

            if (Data == null) { return false; }
            var streamData = new MemoryStream(Data);
            if (streamData.Length == 0) { return false; }

            var currentDir = Directory.GetCurrentDirectory();
            var dirRoot = Directory.GetDirectoryRoot(currentDir);
            var basePath = Path.Combine(dirRoot, "home", "edo"); // Base Path on the Storage

            var fullPath = Path.Combine(basePath, path, signName);

            try
            {
                using var stream = new FileStream(fullPath, FileMode.Create);
                streamData.CopyTo(stream);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool DeleteFolder(string path)
        {
            if (!Directory.Exists(path)) { return false; }

            Directory.Delete(path, true);

            return true;
        }

        public static string NextAvailableFilename(string path)
        {
            // Short-cut if already available
            if (!File.Exists(path)) { return path; }

            // If path has extension then insert the number pattern just before the extension and return next filename
            if (Path.HasExtension(path))
            {
                return GetNextFilename(path.Insert(path.LastIndexOf(Path.GetExtension(path)), numberPattern));
            }

            // Otherwise just append the pattern to the path and return next filename
            return GetNextFilename(path + numberPattern);
        }
        private static string GetNextFilename(string pattern)
        {
            string tmp = string.Format(pattern, 1);
            //if (tmp == pattern)
            //throw new ArgumentException("The pattern must include an index place-holder", "pattern");

            if (!File.Exists(tmp))
                return tmp; // short-circuit if no matches

            int min = 1, max = 2; // min is inclusive, max is exclusive/untested

            while (File.Exists(string.Format(pattern, max)))
            {
                min = max;
                max *= 2;
            }

            while (max != min + 1)
            {
                int pivot = (max + min) / 2;
                if (File.Exists(string.Format(pattern, pivot)))
                    min = pivot;
                else
                    max = pivot;
            }

            return string.Format(pattern, max);
        }
        private static string GetNextUrlName(string storage, string url)
        {
            var i = 1;
            var testUrl = $"{url}_{i}";

            while (Directory.Exists($"{Path.Combine(storage, testUrl)}"))
            {
                i += 1;
                testUrl = $"{url}_{i}";
            }

            return testUrl;
        }
        private static string GetNextArcPath(string storage, string url, string fileTitle)
        {
            var i = 1;
            var testUrl = Path.Combine(url, $"{fileTitle}_{i}.zip");
            var testPath = Path.Combine(storage, testUrl);

            while (File.Exists(testPath))
            {
                i++;
                testUrl = Path.Combine(url, $"{fileTitle}_{i}.zip");
                testPath = Path.Combine(storage, testUrl);
            }

            return testUrl;
        }
        private static string CheckName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) { return ""; }

            foreach (char c in " +=[]:;,./?*<>(){}\\\"")
            {
                if (name.Contains(c)) { name = name.Replace(c, '_'); }
            }

            return name;
        }
    }
}