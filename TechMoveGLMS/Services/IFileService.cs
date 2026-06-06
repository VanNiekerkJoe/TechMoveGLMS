using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace TechMoveGLMS.Services
{
    public interface IFileService
    {
        Task<string> SavePdfFileAsync(IFormFile file, string contractId);
        bool IsValidPdfFile(IFormFile file);
    }
}