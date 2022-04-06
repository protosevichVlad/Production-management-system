using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ProductionManagementSystem.Core.Repositories;
using ProductionManagementSystem.Core.Services.AltiumDB;

namespace ProductionManagementSystem.Core.Services
{
    public interface IImportService
    {
        Task ImportFromFile(string tableName, StreamReader stream, IDataImporter importer);
    }
    
    public class ImportService : IImportService
    {
        private readonly ITableService _tableService;
        private readonly IEntityExtService _entityExtService;

        public ImportService(ITableService tableService, IEntityExtService entityExtService)
        {
            _tableService = tableService;
            _entityExtService = entityExtService;
        }

        public async Task ImportFromFile(string tableName, StreamReader stream, IDataImporter importer)
        {
            await foreach (var table in importer.GetDatabaseTables(tableName, stream))
            {
                var data = await importer.GetData(stream, table);

                try
                {
                    var tableFromDb = await _tableService.GetTableByNameAsync(table.TableName);
                    int tableId = tableFromDb?.Id ?? 0;
                    if (tableFromDb == null)
                    {
                        await _tableService.CreateAsync(table);
                        tableId = table.Id;
                    }
                    
                    //TODO: write sql query for insert list dictionary
                    foreach (var d in data)
                    {
                        d.TableId = tableId;
                        if (_entityExtService.GetEntityExtByPartNumber(d.PartNumber) != null)
                            await _entityExtService.CreateAsync(d);
                        else
                            await _entityExtService.UpdateAsync(d);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(table.DisplayName);
                    Console.WriteLine(e.Message);
                }
                
            }
        }
    }
}