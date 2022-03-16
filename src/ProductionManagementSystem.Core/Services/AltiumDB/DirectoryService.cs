using ProductionManagementSystem.Core.Models.AltiumDB;
using ProductionManagementSystem.Core.Repositories;

namespace ProductionManagementSystem.Core.Services.AltiumDB
{
    public interface IDirectoryService : IBaseService<Directory>
    {
        
    }
    
    public class DirectoryService : BaseService<Directory>, IDirectoryService
    {
        public DirectoryService(IUnitOfWork db) : base(db)
        {
            _currentRepository = _db.DirectoryRepository;
        }
    }
}