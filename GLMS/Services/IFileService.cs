// Code attribution
// OpenAI. 2026. ChatGPT (Version 5.3)
// Used for guidance

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GLMS.Services
{
    public interface IFileService
    {
        bool IsValidPdf(IFormFile file);
        Task<string> SaveFileAsync(IFormFile file, string rootPath);
    }
}
