using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductionManagementSystem.Core.Models.AltiumDB;
using ProductionManagementSystem.Core.Repositories.AltiumDB;

namespace ProductionManagementSystem.Core.Services.AltiumDB
{
    public interface IEntityService : IBaseService<BaseAltiumDbEntity>
    {
        Task<BaseAltiumDbEntity> SearchByPartNumber(string partNumber);
        Task<BaseAltiumDbEntity> SearchByPartNumber(string partNumber, string tableName);
        Task<List<BaseAltiumDbEntity>> SearchByKeyWordAsync(string keyWord, string tableName);
        Task<List<BaseAltiumDbEntity>> SearchByKeyWordAsync(string keyWord);
    }
    
    public class EntityService : BaseService<BaseAltiumDbEntity, IAltiumDBUnitOfWork>, IEntityService
    {
        private IEntityService _entityServiceImplementation;

        public EntityService(IAltiumDBUnitOfWork db) : base(db)
        {
            _currentRepository = _db.AltiumDbEntityRepository;
        }

        public async Task<BaseAltiumDbEntity> SearchByPartNumber(string partNumber)
        {
            return await _db.AltiumDbEntityRepository.GetByPartNumber(partNumber);
        }

        public async Task<BaseAltiumDbEntity> SearchByPartNumber(string partNumber, string tableName)
        {
            var table = (await _db.DatabaseTables.FindAsync(x => x.TableName == tableName, "TableColumns")).FirstOrDefault();
            if (table == null)
                return null;
            return await _db.AltiumDbEntityRepository.GetByPartNumber(table, partNumber);
        }

        public async Task<List<BaseAltiumDbEntity>> SearchByKeyWordAsync(string keyWord, string tableName)
        {
            var table = (await _db.DatabaseTables.FindAsync(x => x.TableName == tableName, "TableColumns")).FirstOrDefault();
            if (table == null)
                return null;
            return await _db.AltiumDbEntityRepository.SearchByKeyWordAsync(table, keyWord);
        }

        public async Task<List<BaseAltiumDbEntity>> SearchByKeyWordAsync(string keyWord)
        {
            throw new System.NotImplementedException();
        }
    }
}