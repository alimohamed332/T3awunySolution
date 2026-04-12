using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Application.Contracts
{
    public interface IFileStorageService
    {
        Task<string> SaveImageAsync(IFormFile? file, string folder);
        void DeleteImage(string imageUrl);
    }
}
