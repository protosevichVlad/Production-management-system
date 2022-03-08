using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using ProductionManagementSystem.Core.Data;
using ProductionManagementSystem.Core.Models.AltiumDB;
using ProductionManagementSystem.Core.Repositories;

namespace ProductionManagementSystem.Core.Services.AltiumDB
{
    public interface IDatabaseService : IBaseService<DatabaseTable>
    {
        Task<bool> TableIsExistsAsync(string tableName);
        Task<DatabaseTable> GetTableByNameAsync(string tableName);
        Task<List<Dictionary<string, object>>> GetDataFromTableAsync(string tableName);
        Task DeleteByTableNameAsync(string tableName);
        Task InsertIntoTableByTableNameAsync(string tableName, IDictionary<string, object> data);
        Task UpdateEntityAsync(string tableName, int id, Dictionary<string, object> data);
        Task<IDictionary<string, object>> GetEntityById(string tableName, int id);
    }

    public class DatabaseService : BaseService<DatabaseTable>, IDatabaseService
    {
        private IMySQLTableHelper _tableHelper;
            
        public DatabaseService(string connectionString) : base(new EFUnitOfWork(connectionString))
        {
            _tableHelper = new MySQLTableHelper(connectionString);
            _currentRepository = _db.DatabaseTableRepository;
        }

        public override async Task CreateAsync(DatabaseTable item)
        {
            if (await this.TableIsExistsAsync(item.TableName)) throw new NotImplementedException();
            await base.CreateAsync(item);
            _tableHelper.CreateTable(item);
        }

        public override async Task DeleteAsync(DatabaseTable item)
        {
            if (!await this.TableIsExistsAsync(item.TableName)) throw new NotImplementedException();
            await base.DeleteAsync(item);
            _tableHelper.DeleteTable(item);
        }

        public async Task<bool> TableIsExistsAsync(string tableName)
        {
            var tables = await _db.DatabaseTableRepository.FindAsync(x => x.TableName == tableName);
            return tables.Count > 0;
        }

        public async Task<DatabaseTable> GetTableByNameAsync(string tableName)
        {
            return (await _db.DatabaseTableRepository.FindAsync(x => x.TableName == tableName, "TableColumns")).FirstOrDefault();
        }

        public async Task<List<Dictionary<string, object>>> GetDataFromTableAsync(string tableName)
        {
            var table = await GetTableByNameAsync(tableName);
            if (table == null) return new List<Dictionary<string, object>>();
            return _tableHelper.GetDataFromTable(table);
        }

        public async Task DeleteByTableNameAsync(string tableName)
        {
            var table = await GetTableByNameAsync(tableName);
            if (table == null) throw new NotImplementedException();
            _tableHelper.DeleteTable(table);
            await base.DeleteAsync(table);
        }

        public async Task InsertIntoTableByTableNameAsync(string tableName, IDictionary<string, object> data)
        {
            var table = await GetTableByNameAsync(tableName);
            if (table == null) throw new NotImplementedException();
            _tableHelper.InsertIntoTable(table, data);
        }

        public async Task UpdateEntityAsync(string tableName,  int id, Dictionary<string, object> data)
        {
            var table = await GetTableByNameAsync(tableName);
            if (table == null) throw new NotImplementedException();
            _tableHelper.UpdateDataInTable(table, id, data);
        }

        public async Task<IDictionary<string, object>> GetEntityById(string tableName, int id)
        {
            var table = await GetTableByNameAsync(tableName);
            if (table == null) throw new NotImplementedException();
            return _tableHelper.GetEntityById(table, id);
        }

        public override async Task UpdateAsync(DatabaseTable newTable)
        {
            var table = await GetTableByNameAsync(newTable.TableName);
            var columnsForDelete = new List<TableColumn>();
            var columnsForAdd = new List<TableColumn>();
            foreach (var newColumn in newTable.TableColumns)
            {
                if (table.TableColumns.All(x => x.ColumnName != newColumn.ColumnName))
                {
                    _tableHelper.AddColumn(newTable, newColumn);
                    columnsForAdd.Add(newColumn);
                }
            }
            
            foreach (var column in table.TableColumns)
            {
                if (newTable.TableColumns.All(x => x.ColumnName != column.ColumnName))
                {
                    _tableHelper.DeleteColumn(newTable, column);
                    columnsForDelete.Add(column);
                }
            }
            
            table.TableColumns.AddRange(columnsForAdd);
            table.TableColumns = table.TableColumns.Except(columnsForDelete).ToList();

            foreach (var column in table.TableColumns)
            {
                var newTableColumn = newTable.TableColumns.FirstOrDefault(x => x.ColumnName == column.ColumnName);
                if (newTableColumn != null)
                {
                    column.Display = newTableColumn.Display;
                }
            }

            await base.UpdateAsync(table);
        }
    }
}