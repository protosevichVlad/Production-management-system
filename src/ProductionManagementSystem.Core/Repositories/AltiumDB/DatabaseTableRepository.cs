using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using ProductionManagementSystem.Core.Data;
using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Models.AltiumDB;

namespace ProductionManagementSystem.Core.Repositories.AltiumDB
{
    public interface IDatabaseTableRepository : IRepository<Table>
    {
        Task<Table> GetTableByNameAsync(string tableName);
        Task<bool> TableIsExistsAsync(string tableName);
    }
    
    public class DatabaseTableRepository : Repository<Table>, IDatabaseTableRepository
    {
        private readonly IMySqlTableHelper _helper; 
        public DatabaseTableRepository(ApplicationContext db) : base(db)
        {
            _helper = new MySqlTableHelper(db.Database.GetDbConnection());
        }
        
        public async Task<Table> GetTableByNameAsync(string tableName)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.TableName == tableName);
        }

        public override async Task<Table> GetByIdAsync(int id)
        {
            return await _dbSet.Include(x => x.TableColumns).FirstOrDefaultAsync(x => x.Id == id);
        }

        public override async Task CreateAsync(Table item)
        {
            if (await this.TableIsExistsAsync(item.TableName)) throw new NotImplementedException();
            _helper.CreateTable(item);
            await base.CreateAsync(item);
        }

        public async Task<bool> TableIsExistsAsync(string tableName)
        {
            var tables = await _db.Tables.Where(x => x.TableName == tableName).ToListAsync();
            return tables.Count > 0;
        }
    }
}