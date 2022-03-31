using System.ComponentModel.DataAnnotations.Schema;
using ProductionManagementSystem.Core.Models.AltiumDB.Projects;

namespace ProductionManagementSystem.Core.Models.AltiumDB
{
    [Table("AltiumDB_EntityInProjects")]
    public class EntityInProject
    {
        public int Id { get; set; }
        public string PartNumber { get; set; }
        public int Quantity { get; set; }
        public string Designator { get; set; }
        
        public Project Project { get; set; }
        public int ProjectId { get; set; }
        
        [NotMapped]
        public BaseAltiumDbEntity Entity { get; set; }
    }
}