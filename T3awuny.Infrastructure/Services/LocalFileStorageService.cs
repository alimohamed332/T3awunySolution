using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Contracts;

namespace T3awuny.Infrastructure.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _webRootPath;

        public LocalFileStorageService(string webRootPath)
        {
            _webRootPath = webRootPath;
        }

        public async Task<string> SaveImageAsync(IFormFile? file, string folder)
        {
            if (file is null)
                throw new Exception("No file provided");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                throw new Exception("Invalid image format");

            if (file.Length > 5 * 1024 * 1024)
                throw new Exception("Image size exceeds 5MB limit");

            var fileName = $"{Guid.NewGuid()}{extension}";
            var folderPath = Path.Combine(_webRootPath, "uploads", folder); // path = wwwroot/uploads/{folder} : folder is either "users" or "products"
            Directory.CreateDirectory(folderPath);

            var fullPath = Path.Combine(folderPath, fileName);
            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/{folder}/{fileName}";
        }

        public void DeleteImage(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;

            var fullPath = Path.Combine(
                _webRootPath,
                imageUrl.TrimStart('/')
                        .Replace('/', Path.DirectorySeparatorChar)
            );

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}


