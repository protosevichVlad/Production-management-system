using System.Collections.Generic;
using System.Threading.Tasks;
using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Repositories;

namespace ProductionManagementSystem.Core.Services
{
    public interface IEntityExtService : IBaseService<EntityExt>
    {
        Task<List<EntityExt>> GetFromTable(int tableId);
        Task<EntityExt> GetEntityExtByPartNumber(string partNumber);
    }

    public class EntityExtService : BaseService<EntityExt, IUnitOfWork>, IEntityExtService
    {
        public EntityExtService(IUnitOfWork db) : base(db)
        {
            _currentRepository = _db.EntityExtRepository;
        }

        public async Task<List<EntityExt>> GetFromTable(int tableId)
        {
            return await _db.EntityExtRepository.GetAllByTableId(tableId);
        }

        public async Task<EntityExt> GetEntityExtByPartNumber(string partNumber)
        {
            return await _db.EntityExtRepository.GetByPartNumber(partNumber);
        }
    }
}