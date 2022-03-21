using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using MySqlConnector;
using ProductionManagementSystem.Core.Data;
using ProductionManagementSystem.Core.Models.AltiumDB;
using ProductionManagementSystem.Core.Repositories;
using Directory = ProductionManagementSystem.Core.Models.AltiumDB.Directory;

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
    }

    public class DatabaseService : BaseService<DatabaseTable>, IDatabaseService
    {
        private IMySqlTableHelper _tableHelper;
            
        public DatabaseService(string connectionString) : base(new EFUnitOfWork(connectionString))
        {
            _tableHelper = new MySqlTableHelper(connectionString);
            _currentRepository = _db.DatabaseTableRepository;
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
            var tables = await _db.DatabaseTableRepository.FindAsync(x => x.TableName == tableName);
            return tables.Count > 0;
        }

        public async Task<DatabaseTable> GetTableByNameAsync(string tableName)
        {
            var table = (await _db.DatabaseTableRepository.FindAsync(x => x.TableName == tableName, "TableColumns")).FirstOrDefault();
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
            _tableHelper.InsertIntoTable(table, data);
        }

        public async Task UpdateEntityAsync(string tableName, string partNumber, BaseAltiumDbEntity data)
        {
            var table = await GetTableByNameAsync(tableName);
            if (table == null) throw new NotImplementedException();
            _tableHelper.UpdateDataInTable(table, partNumber, data);
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
                    await CreateAsync(table);
                    //TODO: write sql query for insert list dictionary
                    foreach (var d in data)
                    {
                        await InsertIntoTableByTableNameAsync(table.TableName, d);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(table.DisplayName);
                    Console.WriteLine(e.Message);
                }
                
            }
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