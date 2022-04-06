using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using ProductionManagementSystem.Core.Models.AltiumDB;

namespace ProductionManagementSystem.Core.Models
{
    [Table("Tables")]
    public class Table
    {
        [Key]
        public int Id { get; set; }
        public string TableName { get; set; }
        public string DisplayName { get; set; }
        public List<TableColumn> TableColumns { get; set; }
        public string FootprintPath { get; set; }
        public string LibraryPath { get; set; }

        public Table()
        {
        }

        public void InitAltiumDB(string tableName)
        {
            DisplayName = tableName;
            TableName = $"AltiumDB {tableName}";
            TableColumns = AltiumDbEntity.Fields.Select((x, i) => new TableColumn()
            {
                ColumnName = x,
                DatabaseOrder = i,
                Display = AltiumDbEntity.DefaultDisplayFalse.All(y => y != x),
            }).ToList();
        }

        public string GetColumns()
        {
            return string.Join(", ", this.TableColumns.Select(column => $"`{column.ColumnName}`"));
        }

        public string GenerateValueBinding()
        {
            return string.Join(", ", this.TableColumns.Select(column => column.ParameterName));
        }

        public string GenerateUpdateBinding()
        {
            return string.Join(", ", this.TableColumns.Select(column => $"`{column.ColumnName}` = {column.ParameterName}"));
        }
        
        public string GenerateCreateTableSqlQuery()
        {
            StringBuilder result = new StringBuilder($"CREATE TABLE `{TableName}` (");
            result.Append(string.Join(", ", this.TableColumns.Select(column => $"`{column.ColumnName}` VARCHAR(255)")));
            result.Append("); ");
            result.Append($"grant select on `{TableName}` to AltiumDBUser;");
            return result.ToString();
        }
    }
}