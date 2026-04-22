// Code attribution
// OpenAI. 2026. ChatGPT (Version 5.3)
// Used for guidance

using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GLMS.Services
{
    public class FileService : IFileService
    {
        public bool IsValidPdf(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;
            var ext = Path.GetExtension(file.FileName).ToLower();
            return ext == ".pdf";
        }

        public async Task<string> SaveFileAsync(IFormFile file, string rootPath)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(rootPath, "files", fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return $"/files/{fileName}";
        }
    }
}
