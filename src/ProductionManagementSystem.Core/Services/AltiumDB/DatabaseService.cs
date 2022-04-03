using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ProductionManagementSystem.Core.Data;
using ProductionManagementSystem.Core.Models.AltiumDB;
using ProductionManagementSystem.Core.Repositories.AltiumDB;

namespace ProductionManagementSystem.Core.Services.AltiumDB
{
    public interface IDatabaseService : IBaseService<DatabaseTable>
    {
        Task<bool> TableIsExistsAsync(string tableName);
        Task<DatabaseTable> GetTableByNameAsync(string tableName);
        Task<List<BaseAltiumDbEntity>> GetDataFromTableAsync(string tableName);
        Task DeleteByTableNameAsync(string tableName);
        Task InsertIntoTableByTableNameAsync(string tableName, BaseAltiumDbEntity data);
        Task UpdateEntityAsync(string tableName, string partNumber, BaseAltiumDbEntity data);
        Task<BaseAltiumDbEntity> GetEntityByPartNumber(string tableName, string partNumber);
        Task DeleteEntityById(string tableName, string partNumber);
        Task ImportFromFile(string tableName, StreamReader stream, IDataImporter importer);
        Task<List<string>> GetFiledTable(string tableName, string filed);
        Task<List<string>> GetFiledFromAllTables(string filed);
    }

    public class DatabaseService : BaseService<DatabaseTable, IAltiumDBUnitOfWork>, IDatabaseService
    {
        private IMySqlTableHelper _tableHelper;
        private IToDoNoteService _toDoNoteService;
            
        public DatabaseService(IAltiumDBUnitOfWork db, IMySqlTableHelper helper, IToDoNoteService toDoNoteService) : base(db)
        {
            _tableHelper = helper;
            _toDoNoteService = toDoNoteService;
            _currentRepository = _db.DatabaseTables;
        }

        public override async Task CreateAsync(DatabaseTable item)
        {
            if (await this.TableIsExistsAsync(item.TableName)) throw new NotImplementedException();
            _tableHelper.CreateTable(item);
            await base.CreateAsync(item);
        }

        public override async Task DeleteAsync(DatabaseTable item)
        {
            if (!await this.TableIsExistsAsync(item.TableName)) throw new NotImplementedException();
            _tableHelper.DeleteTable(item);
            await base.DeleteAsync(item);
        }

        public async Task<bool> TableIsExistsAsync(string tableName)
        {
            var tables = await _db.DatabaseTables.FindAsync(x => x.TableName == tableName);
            return tables.Count > 0;
        }

        public async Task<DatabaseTable> GetTableByNameAsync(string tableName)
        {
            var table = (await _db.DatabaseTables.FindAsync(x => x.TableName == tableName, "TableColumns")).FirstOrDefault();
            if (table == null) throw new NotImplementedException();
            table.TableColumns = table.TableColumns.OrderBy(x => x.DatabaseOrder).ToList();
            return table;
        }

        public async Task<List<BaseAltiumDbEntity>> GetDataFromTableAsync(string tableName)
        {
            var table = await GetTableByNameAsync(tableName);
            if (table == null) return new List<BaseAltiumDbEntity>();
            return _tableHelper.GetDataFromTable(table);
        }

        public async Task DeleteByTableNameAsync(string tableName)
        {
            var table = await GetTableByNameAsync(tableName);
            if (table == null) throw new NotImplementedException();
            _tableHelper.DeleteTable(table);
            await base.DeleteAsync(table);
        }

        public async Task InsertIntoTableByTableNameAsync(string tableName, BaseAltiumDbEntity data)
        {
            var table = await GetTableByNameAsync(tableName);
            if (table == null) throw new NotImplementedException();
            await AddToToDoNotes(table, data);
            _tableHelper.InsertIntoTable(table, data);
        }

        public async Task UpdateEntityAsync(string tableName, string partNumber, BaseAltiumDbEntity data)
        {
            var table = await GetTableByNameAsync(tableName);
            if (table == null) throw new NotImplementedException();
            await AddToToDoNotes(table, data);
            _tableHelper.UpdateDataInTable(table, partNumber, data);
        }

        private async Task AddToToDoNotes(DatabaseTable table, BaseAltiumDbEntity data)
        {
            if (_tableHelper.GetFiledTable(table, "Library Ref").Count(x => data.LibraryRef == x) == 0)
            {
                await _toDoNoteService.CreateAsync(new ToDoNote()
                {
                    Completed = false,
                    CreatedDateTime = DateTime.Now,
                    Description = "",
                    Title = $"Create Library Ref: '{data.LibraryRef}' for {table.DisplayName} table",
                });
            }
            
            if (_tableHelper.GetFiledTable(table, "Footprint Ref").Count(x => data.FootprintRef == x) == 0)
            {
                await _toDoNoteService.CreateAsync(new ToDoNote()
                {
                    Completed = false,
                    CreatedDateTime = DateTime.Now,
                    Description = "",
                    Title = $"Create Footprint Ref: '{data.FootprintRef}' for {table.DisplayName} table",
                });
            }

            await _db.SaveAsync();
        }

        public async Task<BaseAltiumDbEntity> GetEntityByPartNumber(string tableName, string partNumber)
        {
            var table = await GetTableByNameAsync(tableName);
            if (table == null) throw new NotImplementedException();
            return _tableHelper.GetEntityByPartNumber(table, partNumber);
        }

        public async Task DeleteEntityById(string tableName, string partNumber)
        {
            var table = await GetTableByNameAsync(tableName);
            if (table == null) throw new NotImplementedException();
            _tableHelper.DeleteEntity(table, partNumber);
        }

        public async Task ImportFromFile(string tableName, StreamReader stream, IDataImporter importer)
        {
            await foreach (var table in importer.GetDatabaseTables(tableName, stream))
            {
                var data = await importer.GetData(stream, table);

                try
                {
                    if (await GetTableByNameAsync(table.TableName) == null)
                        await CreateAsync(table);
                    //TODO: write sql query for insert list dictionary
                    foreach (var d in data)
                    {
                        if (string.IsNullOrWhiteSpace((await GetEntityByPartNumber(table.TableName, d.PartNumber)).PartNumber))
                            await InsertIntoTableByTableNameAsync(table.TableName, d);
                        else
                            await UpdateEntityAsync(table.TableName, d.PartNumber, d);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(table.DisplayName);
                    Console.WriteLine(e.Message);
                }
                
            }
        }

        public async Task<List<string>> GetFiledFromAllTables(string filed)
        {
            return _tableHelper.GetFiledFromAllTables(await GetAllAsync(), filed).OrderBy(x => x).ToList();
        }

        public async Task<List<string>> GetFiledTable(string tableName, string filed)
        {
            return _tableHelper.GetFiledTable(await GetTableByNameAsync(tableName), filed).OrderBy(x => x).ToList();
        }

        public override async Task UpdateAsync(DatabaseTable newTable)
        {
            var table = await GetTableByNameAsync(newTable.TableName);
            var columnsForDelete = new List<TableColumn>();
            var columnsForAdd = new List<TableColumn>();
            foreach (var newColumn in newTable.TableColumns)
            {
                if (table.TableColumns.All(x => x.Id != newColumn.Id))
                {
                    _tableHelper.AddColumn(newTable, newColumn);
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
                            _tableHelper.RenameColumn(table, table.TableColumns[indexForRename], newColumn);
                        table.TableColumns[indexForRename].ColumnName = newColumn.ColumnName;
                        table.TableColumns[indexForRename].Display = newColumn.Display;
                        table.TableColumns[indexForRename].DatabaseOrder = newColumn.DatabaseOrder;
                    }
                }
            }
            
            foreach (var column in table.TableColumns)
            {
                if (newTable.TableColumns.All(x => x.Id != column.Id))
                {
                    _tableHelper.DeleteColumn(newTable, column);
                    columnsForDelete.Add(column);
                }
            }
            
            table.TableColumns.AddRange(columnsForAdd);
            table.TableColumns = table.TableColumns.Except(columnsForDelete).ToList();

            foreach (var column in table.TableColumns)
            {
                var newTableColumn = newTable.TableColumns.FirstOrDefault(x => x.Id == column.Id);
                if (newTableColumn != null)
                {
                    column.Display = newTableColumn.Display;
                }
            }

            table.DirectoryId = newTable.DirectoryId;
            table.DisplayName = newTable.DisplayName;
            if (table.FootprintPath != newTable.FootprintPath)
            {
                table.FootprintPath = newTable.FootprintPath;
                _tableHelper.UpdateLibraryPropertyInTable(table, "Footprint Path", newTable.FootprintPath);
            }
            if (table.LibraryPath != newTable.LibraryPath)
            {
                table.LibraryPath = newTable.LibraryPath;
                _tableHelper.UpdateLibraryPropertyInTable(table, "Library Path", newTable.LibraryPath);
            }
            await base.UpdateAsync(table);
        }
    }
}