using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MySqlConnector;
using OfficeOpenXml;
using ProductionManagementSystem.Core.Models.AltiumDB;

namespace ProductionManagementSystem.Core.Services.AltiumDB
{
    public interface IDataImporter
    {
        IAsyncEnumerable<DatabaseTable> GetDatabaseTables(string tableName, StreamReader streamReader);
        Task<List<Dictionary<string, object>>> GetData(StreamReader streamReader, DatabaseTable table);
    }

    public class CsvDataImporter : IDataImporter
    {
        public async IAsyncEnumerable<DatabaseTable> GetDatabaseTables(string tableName, StreamReader streamReader)
        {
            DatabaseTable table = new DatabaseTable() {DisplayName = tableName, TableName = $"AltiumDB_{tableName}", TableColumns = new List<TableColumn>()};
            table.TableColumns.Add(new TableColumn(){ColumnName = "Id", ColumnType = MySqlDbType.Int32});
            var line = await streamReader.ReadLineAsync();
            if (line == null) throw new NotImplementedException();
            if (table.TableColumns.Count <= 1)
            {
                var columnsName = line.Split(',');
                foreach (var columnName in columnsName)
                {
                    table.TableColumns.Add(new TableColumn(){ColumnName = columnName.Replace(' ', '_'), ColumnType = MySqlDbType.String, Display = true});
                }
            }

            yield return table;
        }

        public async Task<List<Dictionary<string, object>>> GetData(StreamReader streamReader, DatabaseTable table)
        {
            List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
            while (true)
            {
                var line = await streamReader.ReadLineAsync();
                if (line == null) break;
                data.Add(new Dictionary<string, object>());
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
        public async IAsyncEnumerable<DatabaseTable> GetDatabaseTables(string tableName, StreamReader streamReader)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    
            using(var package = new ExcelPackage(streamReader.BaseStream))
            {
                foreach (var worksheet in package.Workbook.Worksheets)
                {
                    tableName = worksheet.Name;
                    DatabaseTable table = new DatabaseTable() {DisplayName = tableName, TableName = $"AltiumDB_{tableName.Replace(' ', '_')}", TableColumns = new List<TableColumn>()};
                    table.TableColumns.Add(new TableColumn(){ColumnName = "Id", ColumnType = MySqlDbType.Int32});
                    int i = 1;
                    while(!string.IsNullOrWhiteSpace(worksheet.Cells[1, i].Text))
                    {
                        var columnName = worksheet.Cells[1, i].Text;
                        table.TableColumns.Add(new TableColumn(){ColumnName = columnName.Replace(' ', '_'), ColumnType = MySqlDbType.String, Display = true});
                        i++;
                    }
                    
                    yield return table;
                }
            }
        }

        public async Task<List<Dictionary<string, object>>> GetData(StreamReader streamReader, DatabaseTable table)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            using(var package = new ExcelPackage(streamReader.BaseStream))
            {
                List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
                var worksheet = package.Workbook.Worksheets[table.DisplayName];
                for (int rowIndex = 2;; rowIndex++)
                {
                    var rowData = new Dictionary<string, object>();
                    foreach (var (column, i) in table.TableColumns.Where(x => x.ColumnName != "Id").Select((x, i) => (x, i)))
                    {
                        rowData[column.ColumnName] = worksheet.Cells[rowIndex, i+1].Text;
                    }

                    if (rowData.Values.All(x => string.IsNullOrWhiteSpace(x.ToString()))) break;
                    data.Add(rowData);
                }

                return data;
            }
        }
    }
}