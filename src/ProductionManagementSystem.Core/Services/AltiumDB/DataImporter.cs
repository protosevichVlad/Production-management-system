using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MySqlConnector;
using OfficeOpenXml;
using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Models.AltiumDB;

namespace ProductionManagementSystem.Core.Services.AltiumDB
{
    public interface IDataImporter
    {
        IAsyncEnumerable<Table> GetDatabaseTables(string tableName, StreamReader streamReader);
        Task<List<EntityExt>> GetData(StreamReader streamReader, Table table);
    }

    public class CsvDataImporter : IDataImporter
    {
        public async IAsyncEnumerable<Table> GetDatabaseTables(string tableName, StreamReader streamReader)
        {
            Table table = new Table();
            table.InitAltiumDB(tableName);
            
            var line = await streamReader.ReadLineAsync();
            if (line == null) throw new NotImplementedException();
            if (table.TableColumns.Count <= 1)
            {
                var columnsName = line.Split(',');
                foreach (var (columnName, i) in columnsName.Select((x, i) => (x, i)))
                {
                    var columnPosition = table.TableColumns.FindIndex(x => x.ColumnName == columnName);
                    if (columnPosition == -1)
                    {
                        table.TableColumns.Add(new TableColumn()
                        {
                            ColumnName = columnName, 
                            Display = true, 
                            DatabaseOrder = table.TableColumns.Count
                        });
                    }
                }
            }

            yield return table;
        }

        public async Task<List<EntityExt>> GetData(StreamReader streamReader, Table table)
        {
            List<EntityExt> data = new List<EntityExt>();
            table.TableColumns = table.TableColumns.OrderBy(x => x.DatabaseOrder).ToList();
            while (true)
            {
                var line = await streamReader.ReadLineAsync();
                if (line == null) break;
                data.Add(new EntityExt());
                var values = line.Split(',');
                for (int i = 1; i < table.TableColumns.Count; i++)
                {
                    data[^1][table.TableColumns[i].ColumnName] = values[i - 1];
                }
            }

            return data;
        }
    }

    public class ExcelImporter : IDataImporter
    {
        public async IAsyncEnumerable<Table> GetDatabaseTables(string tableName, StreamReader streamReader)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    
            using(var package = new ExcelPackage(streamReader.BaseStream))
            {
                foreach (var worksheet in package.Workbook.Worksheets)
                {
                    tableName = worksheet.Name;
                    Table table = new Table();
                    table.InitAltiumDB(tableName);
                    
                    int i = 1;
                    while(!string.IsNullOrWhiteSpace(worksheet.Cells[1, i].Text))
                    {
                        var columnName = worksheet.Cells[1, i].Text;
                        var columnPosition = table.TableColumns.FindIndex(x => x.ColumnName == columnName);
                        if (columnPosition == -1)
                        {
                            table.TableColumns.Add(new TableColumn()
                            {
                                ColumnName = columnName, 
                                Display = true, 
                                DatabaseOrder = table.TableColumns.Count
                            });
                        }
                        i++;
                    }
                    
                    yield return table;
                }
            }
        }

        public async Task<List<EntityExt>> GetData(StreamReader streamReader, Table table)
        {
            table.TableColumns = table.TableColumns.OrderBy(x => x.DatabaseOrder).ToList();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using(var package = new ExcelPackage(streamReader.BaseStream))
            {
                List<EntityExt> data = new List<EntityExt>();
                var worksheet = package.Workbook.Worksheets[table.DisplayName];
                for (int rowIndex = 2;; rowIndex++)
                {
                    var rowData = new EntityExt();
                    for (int i = 0; i < table.TableColumns.Count; i++)
                    {
                        rowData[worksheet.Cells[1, i+1].Text] = worksheet.Cells[rowIndex, i+1].Text;
                    }

                    if (rowData.Values.All(x => string.IsNullOrWhiteSpace(x.ToString()))) break;
                    data.Add(rowData);
                }

                return data;
            }
        }
    }
}