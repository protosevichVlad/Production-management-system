using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
    }
}