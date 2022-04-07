using System.Collections.Generic;
using System.Threading.Tasks;
using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Models.AltiumDB;
using ProductionManagementSystem.Core.Repositories;
using ProductionManagementSystem.Core.Repositories.AltiumDB;

namespace ProductionManagementSystem.Core.Services.AltiumDB
{
    public interface ITableService : IBaseService<Table>
    {
        Task<List<Table>> SearchByKeyWordAsync(string keyWord);
        Task<Table> GetTableByNameAsync(string tableName);
        Task DeleteByIdAsync(int id);
    }

    public class TableService : BaseService<Table, IUnitOfWork>, ITableService
    {
        public TableService(IUnitOfWork db) : base(db)
        {
            _currentRepository = _db.DatabaseTableRepository;
        }

        public async Task<List<Table>> SearchByKeyWordAsync(string keyWord)
        {
            if (string.IsNullOrEmpty(keyWord))
                return new List<Table>();
            return await _db.DatabaseTableRepository.FindAsync(x =>
                !string.IsNullOrEmpty(x.DisplayName) && x.DisplayName.Contains(keyWord));
        }

        public async Task<Table> GetTableByNameAsync(string tableName)
        {
            return await _db.DatabaseTableRepository.GetTableByNameAsync(tableName);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var table = await _currentRepository.GetByIdAsync(id);
            _currentRepository.Delete(table);
            await _db.SaveAsync();
        }
    }
}