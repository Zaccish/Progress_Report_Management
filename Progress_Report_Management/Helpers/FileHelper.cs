// Helpers/FileHelper.cs
using Microsoft.AspNetCore.Http;
using System.IO;
using ProgressReportSystem.API.Helpers;


namespace ProgressReportSystem.API.Helpers
{
    public static class FileHelper
    {
        private static readonly string[] permittedExtensions = { ".pdf", ".docx", ".doc" };

        public static bool IsValidReportFile(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            return !string.IsNullOrEmpty(ext) && permittedExtensions.Contains(ext);
        }

        public static string SaveFile(IFormFile file, string rootPath)
        {
            var uploadsFolder = Path.Combine(rootPath, "UploadedReports");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return filePath;
        }
    }
}
