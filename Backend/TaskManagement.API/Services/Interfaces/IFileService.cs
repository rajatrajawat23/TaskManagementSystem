using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace TaskManagement.API.Services.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file, string folderPath);
        Task<bool> DeleteFileAsync(string filePath);
        Task<byte[]> GetFileAsync(string filePath);
        string GetContentType(string fileName);
        bool IsValidFileExtension(string fileName);
        bool IsValidFileSize(long fileSize);
    }
}