using Microsoft.AspNetCore.Http;
using TaskManagement.API.Services.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace TaskManagement.API.Services.Implementation
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileService> _logger;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".zip" };
        private readonly long _maxFileSize = 10 * 1024 * 1024; // 10MB

        public FileService(IWebHostEnvironment environment, ILogger<FileService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderPath)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("File is empty");

                if (!IsValidFileExtension(file.FileName))
                    throw new ArgumentException($"File extension not allowed. Allowed extensions: {string.Join(", ", _allowedExtensions)}");

                if (!IsValidFileSize(file.Length))
                    throw new ArgumentException($"File size exceeds the maximum allowed size of {_maxFileSize / (1024 * 1024)}MB");

                // Create upload directory
                var uploadsFolder = Path.Combine(_environment.WebRootPath ?? "wwwroot", "uploads", folderPath);
                
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Generate unique file name
                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Return relative URL
                var relativeUrl = $"/uploads/{folderPath}/{uniqueFileName}".Replace("\\", "/");
                _logger.LogInformation($"File uploaded successfully: {relativeUrl}");
                
                return relativeUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                throw;
            }
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return false;

                // Convert URL to physical path
                var relativePath = filePath.TrimStart('/');
                var physicalPath = Path.Combine(_environment.WebRootPath ?? "wwwroot", relativePath);

                if (File.Exists(physicalPath))
                {
                    await Task.Run(() => File.Delete(physicalPath));
                    _logger.LogInformation($"File deleted successfully: {filePath}");
                    return true;
                }

                _logger.LogWarning($"File not found for deletion: {filePath}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting file: {filePath}");
                return false;
            }
        }

        public async Task<byte[]> GetFileAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    throw new ArgumentException("File path is empty");

                // Convert URL to physical path
                var relativePath = filePath.TrimStart('/');
                var physicalPath = Path.Combine(_environment.WebRootPath ?? "wwwroot", relativePath);

                if (!File.Exists(physicalPath))
                    throw new FileNotFoundException($"File not found: {filePath}");

                return await File.ReadAllBytesAsync(physicalPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error reading file: {filePath}");
                throw;
            }
        }

        public string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".txt" => "text/plain",
                ".zip" => "application/zip",
                _ => "application/octet-stream"
            };
        }

        public bool IsValidFileExtension(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return _allowedExtensions.Contains(extension);
        }

        public bool IsValidFileSize(long fileSize)
        {
            return fileSize > 0 && fileSize <= _maxFileSize;
        }
    }
}
