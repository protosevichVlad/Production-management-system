using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductionManagementSystem.Core.Models.AltiumDB;
using ProductionManagementSystem.Core.Repositories.AltiumDB;

namespace ProductionManagementSystem.Core.Services.AltiumDB
{
    public interface IEntityService : IBaseService<AltiumDbEntity>
    {
        Task<AltiumDbEntity> SearchByPartNumber(string partNumber);
        Task<AltiumDbEntity> SearchByPartNumber(string partNumber, string tableName);
        Task<List<AltiumDbEntity>> SearchByKeyWordAsync(string keyWord, string tableName);
        Task<List<AltiumDbEntity>> SearchByKeyWordAsync(string keyWord);
    }
    
    public class EntityService : BaseService<AltiumDbEntity, IAltiumDBUnitOfWork>, IEntityService
    {
        private IEntityService _entityServiceImplementation;

        public EntityService(IAltiumDBUnitOfWork db) : base(db)
        {
            _currentRepository = _db.AltiumDbEntityRepository;
        }

        public async Task<AltiumDbEntity> SearchByPartNumber(string partNumber)
        {
            return await _db.AltiumDbEntityRepository.GetByPartNumber(partNumber);
        }

        public async Task<AltiumDbEntity> SearchByPartNumber(string partNumber, string tableName)
        {
            var table = (await _db.DatabaseTables.FindAsync(x => x.TableName == tableName, "TableColumns")).FirstOrDefault();
            if (table == null)
                return null;
            return await _db.AltiumDbEntityRepository.GetByPartNumber(table, partNumber);
        }

        public async Task<List<AltiumDbEntity>> SearchByKeyWordAsync(string keyWord, string tableName)
        {
            var table = (await _db.DatabaseTables.FindAsync(x => x.TableName == tableName, "TableColumns")).FirstOrDefault();
            if (table == null)
                return null;
            return await _db.AltiumDbEntityRepository.SearchByKeyWordAsync(table, keyWord);
        }

        public async Task<List<AltiumDbEntity>> SearchByKeyWordAsync(string keyWord)
        {
            return await _db.AltiumDbEntityRepository.SearchByKeyWordAsync(keyWord);
        }
    }
}