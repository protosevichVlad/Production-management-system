using System;
using System.Text;
using System.Threading.Tasks;
using ProductionManagementSystem.Core.Models.AltiumDB;
using ProductionManagementSystem.Core.Repositories;

namespace ProductionManagementSystem.Core.Services.AltiumDB
{
    public interface IDirectoryService : IBaseService<Directory>
    {
        Task<string> GetPathByDirectoryIdAsync(int id);
        Task<string> GetPathByTableIdAsync(int id);
    }
    
    public class DirectoryService : BaseService<Directory>, IDirectoryService
    {
        public DirectoryService(IUnitOfWork db) : base(db)
        {
            _currentRepository = _db.DirectoryRepository;
        }

        public async Task<string> GetPathByDirectoryIdAsync(int id)
        {
            StringBuilder result = new StringBuilder();
            var currentDirectory = await this.GetByIdAsync(id);
            while (currentDirectory != null)
            {
                result.Insert(0, currentDirectory.DirectoryName);
                result.Insert(0, '/');
                currentDirectory = await GetByIdAsync(currentDirectory.ParentId);
            }

            return result.ToString();
        }

        public async Task<string> GetPathByTableIdAsync(int id)
        {
            StringBuilder result = new StringBuilder();
            var table = await _db.DatabaseTableRepository.GetByIdAsync(id);
            if (table == null || !table.DirectoryId.HasValue)
                return "";
            
            result.Append(await GetPathByDirectoryIdAsync(table.DirectoryId.Value));
            result.Append('/');
            result.Append(table.DisplayName);
            return result.ToString();
        }
    }
}