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
        public bool Display { get; set; }
        public int DatabaseTableId { get; set; }
        public DatabaseTable DatabaseTable { get; set; }
        public int DatabaseOrder { get; set; }


        public string ParameterName => "@" + string.Join(string.Empty, ColumnName.Split('@', ' ', '-', ':', '(', ')', '/', '±', ',', '.', ';', '\''));

        public bool Equals(TableColumn other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
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
            return Id;
        }
    }
}