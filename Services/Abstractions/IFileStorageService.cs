using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Abstractions
{
    public interface IFileStorageService
    {
        Task<List<string>> UploadAsync(List<IFormFile> files);
        Task DeleteAsync(string fileUrl);
    }
}
