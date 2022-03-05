using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MySqlConnector;

namespace ProductionManagementSystem.Core.Models.AltiumDB
{
    [Table("TableColumns")]
    public class TableColumn : IEquatable<TableColumn>
    {
        [Key]
        public int Id { get; set; }
        public string ColumnName { get; set; }
        public MySqlDbType ColumnType { get; set; }
        public bool Display { get; set; }
        public int DatabaseTableId { get; set; }
        public DatabaseTable DatabaseTable { get; set; }

        public bool Equals(TableColumn other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ColumnName == other.ColumnName && ColumnType == other.ColumnType && DatabaseTableId == other.DatabaseTableId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TableColumn) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ColumnName, (int) ColumnType, DatabaseTableId);
        }
    }
}