using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace TechMoveGLMS.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public bool IsValidPdfFile(IFormFile file)
        {
            if (file == null || file.Length == 0) return false;
            var ext = Path.GetExtension(file.FileName).ToLower();
            return ext == ".pdf" && file.ContentType == "application/pdf";
        }

        public async Task<string> SavePdfFileAsync(IFormFile file, string contractId)
        {
            if (!IsValidPdfFile(file))
                throw new InvalidOperationException("Only PDF files are allowed.");

            string uploadFolder = Path.Combine(_env.WebRootPath, "contracts");
            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            string uniqueName = $"{contractId}_{Guid.NewGuid()}.pdf";
            string filePath = Path.Combine(uploadFolder, uniqueName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/contracts/{uniqueName}";
        }
    }
}