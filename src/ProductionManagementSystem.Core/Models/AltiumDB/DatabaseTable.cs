using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using MySqlConnector;

namespace ProductionManagementSystem.Core.Models.AltiumDB
{
    [Table("DatabaseTables")]
    public class DatabaseTable
    {
        [Key]
        public int Id { get; set; }
        public string TableName { get; set; }
        public string DisplayName { get; set; }
        public List<TableColumn> TableColumns { get; set; }

        public DatabaseTable()
        {
        }

        public void InitAltiumDB(string tableName)
        {
            DisplayName = tableName;
            TableName = $"AltiumDB_{tableName}";
            TableColumns = new List<TableColumn>
            {
                new TableColumn()
                {
                    ColumnName = "KeyID",
                    DisplayingColumnName = "KeyID",
                    ColumnType = MySqlDbType.Int32,
                    DatabaseOrder = 0
                }
            };
        }

        public string GetColumns()
        {
            StringBuilder result = new StringBuilder("");
            foreach (var column in TableColumns.Where(x => x.ColumnName != "KeyID"))
            {
                result.Append($"`{column.ColumnName}`");
                result.Append(", ");
            }

            result.Remove(result.Length - 2, 2);
            return result.ToString();
        }

        public string GenerateValueBinding()
        {
            StringBuilder result = new StringBuilder("");
            foreach (var column in TableColumns.Where(x => x.ColumnName != "KeyID"))
            {
                result.Append(column.ParameterName);
                result.Append(", ");
            }

            result.Remove(result.Length - 2, 2);
            return result.ToString();
        }

        public string GenerateUpdateBinding()
        {
            StringBuilder result = new StringBuilder("");
            foreach (var column in TableColumns.Where(x => x.ColumnName != "KeyID"))
            {
                result.Append($"`{column.ColumnName}` = {column.ParameterName}, ");
            }
            result.Remove(result.Length - 2, 2);
            return result.ToString();
        }
    }
}