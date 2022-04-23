using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ProductionManagementSystem.Core.Services
{
    public interface IFileService
    {
        Task<string> CreateFileByUrl(Stream stream, string url, string fileName);
        Task<string> CreateFileByUrl(byte[] content, string url, string fileName);
        Task DeleteFileByUrl(string path);
        Task<string> GetNewUrl(string newPath, string oldPath);
    }
    
    public class FileService : IFileService
    {
        public async Task<string> CreateFileByUrl(Stream stream, string url, string fileName)
        {
            if (stream == null)
            {
                return String.Empty;
            }

            var path = GetPathByUrl(url);
            var filePath = Path.Combine(path, fileName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            await using FileStream outputFileStream = new FileStream(filePath, FileMode.Create);
            await stream.CopyToAsync(outputFileStream);

            return GetUrlByPath(filePath);
        }

        public async Task<string> CreateFileByUrl(byte[] content, string url, string fileName)
        {
            if (content == null) return String.Empty;
            await using var stream = new MemoryStream(content);
            return await CreateFileByUrl(stream, url, fileName);
        }

        public async Task DeleteFileByUrl(string url)
        {
            var path = GetPathByUrl(url);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))  
            {  
                File.Delete(path);
            }
        }

        private string GetPathByUrl(string url)
        {
            if  (string.IsNullOrEmpty(url)) return String.Empty;
            return Path.Combine(("wwwroot" + url).Split('/'));
        }
        
        private string GetUrlByPath(string path)
        {
            if  (string.IsNullOrEmpty(path)) return String.Empty;
                return "/" + string.Join("/", path.Split(Path.DirectorySeparatorChar)[1..]);
        }
        
        public async Task<string> GetNewUrl(string newPath, string oldPath)
        {
            if (!string.IsNullOrEmpty(newPath))
            {
                if (!string.IsNullOrEmpty(oldPath) && oldPath != newPath)
                {
                    await DeleteFileByUrl(oldPath);
                }

                return newPath;
            }

            return oldPath;
        }
    }
}