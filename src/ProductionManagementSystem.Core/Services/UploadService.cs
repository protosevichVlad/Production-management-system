using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ProductionManagementSystem.Core.Services
{
    public interface IFileService
    {
        Task UploadFile(Stream stream, string path, string fileName);
        Task DeleteFile(string path);
        Task UploadFiles(List<Stream> stream, string path, List<string> fileNames);
    }
    
    public class FileService : IFileService
    {
        public async Task UploadFile(Stream stream, string path, string fileName)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (File.Exists(Path.Combine(path, fileName)))
            {
                File.Delete(Path.Combine(path ,fileName));
            }

            await using FileStream outputFileStream = new FileStream(Path.Combine(path ,fileName), FileMode.Create);
            await stream.CopyToAsync(outputFileStream);
        }

        public async Task DeleteFile(string path)
        {
            throw new NotImplementedException();
        }

        public async Task UploadFiles(List<Stream> stream, string path, List<string> fileNames)
        {
            throw new System.NotImplementedException();
        }
    }
}