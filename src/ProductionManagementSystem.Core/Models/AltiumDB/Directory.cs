using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductionManagementSystem.Core.Models.AltiumDB
{
    [Table("AltiumDB_Directories")]
    public class Directory
    { 
        public int Id { get; set; }
        public string DirectoryName { get; set; }
        public int ParentId { get; set; }
        [NotMapped]
        public List<Directory> Childs { get; set; }
        public List<DatabaseTable> Tables { get; set; }
    }
}