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
                int tableId = 0;
                try
                {
                    var tableFromDb = await _tableService.GetTableByNameAsync(table.TableName);
                    tableId = tableFromDb?.Id ?? 0;
                    if (tableFromDb == null)
                    {
                        await _tableService.CreateAsync(table);
                        tableId = table.Id;
                    }

                    foreach (var d in data)
                    {
                        try
                        {
                            d.TableId = tableId;
                            var entityInDb = await _entityExtService.GetEntityExtByPartNumber(d.PartNumber);
                            if (entityInDb == null)
                            {
                                d.KeyId = 0;
                                d.Quantity = 0;
                                d.ImageUrl = String.Empty;
                                await _entityExtService.CreateAsync(d);
                            }
                            else
                            {
                                d.KeyId = entityInDb.KeyId;
                                d.Quantity = entityInDb.Quantity;
                                d.ImageUrl = entityInDb.ImageUrl;
                                await _entityExtService.UpdateAsync(d);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"{table.DisplayName} {d.Category} {d.Item} {d.PartNumber}");
                            Console.WriteLine(e.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(table.DisplayName);
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}