using System;
using System.Collections.Generic;
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
            var table = await _dbSet.Include(x => x.TableColumns).FirstOrDefaultAsync(x => x.TableName == tableName);
            table.TableColumns = table.TableColumns.OrderBy(x => x.DatabaseOrder).ToList();
            return table;
        }

        public override async Task<Table> GetByIdAsync(int id)
        {
            var table = await _dbSet.Include(x => x.TableColumns).FirstOrDefaultAsync(x => x.Id == id);
            table.TableColumns = table.TableColumns.OrderBy(x => x.DatabaseOrder).ToList();
            return table;
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

        public override void Delete(Table item)
        {
            _helper.DeleteTable(item);
            _dbSet.Remove(item);
        }

        public override async Task UpdateAsync(Table item)
        {
            var table = await GetTableByNameAsync(item.TableName);
            var columnsForDelete = new List<TableColumn>();
            var columnsForAdd = new List<TableColumn>();
            foreach (var newColumn in item.TableColumns)
            {
                if (table.TableColumns.All(x => x.Id != newColumn.Id))
                {
                    _helper.AddColumn(item, newColumn);
                    columnsForAdd.Add(newColumn);
                }
                else
                {
                    int indexForRename = table.TableColumns.FindIndex(x =>
                        x.Id == newColumn.Id && 
                        (x.ColumnName != newColumn.ColumnName || x.Display != newColumn.Display ||
                         x.DatabaseOrder != newColumn.DatabaseOrder));
                    if (indexForRename != -1)
                    {
                        if (table.TableColumns[indexForRename].ColumnName != newColumn.ColumnName)
                            _helper.RenameColumn(table, table.TableColumns[indexForRename], newColumn);
                        table.TableColumns[indexForRename].ColumnName = newColumn.ColumnName;
                        table.TableColumns[indexForRename].Display = newColumn.Display;
                        table.TableColumns[indexForRename].DatabaseOrder = newColumn.DatabaseOrder;
                    }
                }
            }
            
            foreach (var column in table.TableColumns)
            {
                if (item.TableColumns.All(x => x.Id != column.Id))
                {
                    _helper.DeleteColumn(item, column);
                    columnsForDelete.Add(column);
                }
            }
            
            table.TableColumns.AddRange(columnsForAdd);
            table.TableColumns = table.TableColumns.Except(columnsForDelete).ToList();

            foreach (var column in table.TableColumns)
            {
                var newTableColumn = item.TableColumns.FirstOrDefault(x => x.Id == column.Id);
                if (newTableColumn != null)
                {
                    column.Display = newTableColumn.Display;
                }
            }

            table.DisplayName = item.DisplayName;
            if (table.FootprintPath != item.FootprintPath)
            {
                table.FootprintPath = item.FootprintPath;
                _helper.UpdateLibraryPropertyInTable(table, "Footprint Path", item.FootprintPath);
            }
            if (table.LibraryPath != item.LibraryPath)
            {
                table.LibraryPath = item.LibraryPath;
                _helper.UpdateLibraryPropertyInTable(table, "Library Path", item.LibraryPath);
            }
            await base.UpdateAsync(table);
        }
    }
}