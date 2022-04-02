using System.Collections.Generic;
using System.Threading.Tasks;
using ProductionManagementSystem.Core.Models.AltiumDB;
using ProductionManagementSystem.Core.Repositories;
using ProductionManagementSystem.Core.Repositories.AltiumDB;

namespace ProductionManagementSystem.Core.Services.AltiumDB
{
    public interface ITableService : IBaseService<DatabaseTable>
    {
        Task<List<DatabaseTable>> SearchByKeyWordAsync(string keyWord);
    }

    public class TableService : BaseService<DatabaseTable, IAltiumDBUnitOfWork>, ITableService
    {
        public TableService(IAltiumDBUnitOfWork db) : base(db)
        {
            _currentRepository = _db.DatabaseTables;
        }

        public async Task<List<DatabaseTable>> SearchByKeyWordAsync(string keyWord)
        {
            if (string.IsNullOrEmpty(keyWord))
                return new List<DatabaseTable>();
            return await _db.DatabaseTables.FindAsync(x =>
                !string.IsNullOrEmpty(x.DisplayName) && x.DisplayName.Contains(keyWord));
        }
    }
}